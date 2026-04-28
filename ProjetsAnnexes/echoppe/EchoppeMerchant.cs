using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DOL.Database;
using DOL.GS.Housing;
using DOL.GS.PacketHandler;
using log4net;

namespace DOL.GS.Custom.Echoppe
{
    public class EchoppeMerchant : GameNPC, IGameInventoryObject
    {
        private static new readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public const int CONSIGNMENT_SIZE = 100;
        public const int CONSIGNMENT_OFFSET = 1350;
        public const string ITEM_BEING_ADDED = "ItemBeingAddedToObject";
        public const string CONSIGNMENT_BUY_ITEM = "ConsignmentBuyItem";

        protected Dictionary<string, GamePlayer> _observers = [];
        protected long _money;
        protected object _moneyLock = new();

        protected GameTimer _despawnTimer;

        public string OwnerID { get; set; }
        public GamePlayer PlayerOwner { get; set; }

        public virtual eInventorySlot FirstClientSlot => eInventorySlot.HousingInventory_First;
        public virtual eInventorySlot LastClientSlot => eInventorySlot.HousingInventory_Last;
        public virtual int FirstDbSlot => (int)eInventorySlot.Consignment_First;
        public virtual int LastDbSlot => (int)eInventorySlot.Consignment_Last;

        public object LockObject { get; } = new();

        public virtual string GetOwner(GamePlayer player)
        {
            return OwnerID;
        }

        public virtual Dictionary<int, DbInventoryItem> GetClientInventory(GamePlayer player)
        {
            return this.GetClientItems(player);
        }

        public virtual IList<DbInventoryItem> DBItems(GamePlayer player = null)
        {
            return MarketCache.Items.Where(item => item?.OwnerID == OwnerID).ToList();
        }

        public virtual long TotalMoney
        {
            get
            {
                lock (_moneyLock)
                {
                    return _money;
                }
            }
            set
            {
                lock (_moneyLock)
                {
                    _money = value;
                    DbHouseConsignmentMerchant merchant = DOLDB<DbHouseConsignmentMerchant>.SelectObject(DB.Column("OwnerID").IsEqualTo(OwnerID).And(DB.Column("HouseNumber").IsEqualTo(0)));
                    if (merchant != null)
                    {
                        merchant.Money = _money;
                        GameServer.Database.SaveObject(merchant);
                    }
                }
            }
        }

        public virtual bool HasPermissionToMove(GamePlayer player)
        {
            return player != null && player.ObjectId == OwnerID;
        }

        public virtual bool CanHandleMove(GamePlayer player, eInventorySlot fromClientSlot, eInventorySlot toClientSlot)
        {
            return player != null && player.ActiveInventoryObject == this && this.CanHandleRequest(fromClientSlot, toClientSlot);
        }

        public virtual bool MoveItem(GamePlayer player, eInventorySlot fromClientSlot, eInventorySlot toClientSlot, ushort count)
        {
            if (fromClientSlot == toClientSlot)
                return false;

            if (!CanHandleMove(player, fromClientSlot, toClientSlot))
                return false;

            lock (LockObject)
            {
                if (GameInventoryObjectExtensions.IsHousingInventorySlot(fromClientSlot))
                {
                    if (GameInventoryObjectExtensions.IsHousingInventorySlot(toClientSlot))
                    {
                        if (HasPermissionToMove(player))
                            GameInventoryObjectExtensions.NotifyObservers(this, player, _observers, GameInventoryObjectExtensions.MoveItem(this, player, fromClientSlot, toClientSlot, count));
                        else
                            return false;
                    }
                    else
                    {
                        DbInventoryItem toItem = player.Inventory.GetItem(toClientSlot);

                        if (toItem != null)
                        {
                            player.Client.Out.SendMessage("You can only move an item to an empty slot!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                            return false;
                        }

                        if (HasPermissionToMove(player) == false)
                        {
                            OnPlayerBuy(player, fromClientSlot, toClientSlot);
                        }
                        else if (player.TargetObject == this)
                        {
                            GameInventoryObjectExtensions.NotifyObservers(this, player, _observers, GameInventoryObjectExtensions.MoveItem(this, player, fromClientSlot, toClientSlot, count));
                        }
                        else
                        {
                            player.Client.Out.SendMessage("You can't buy items from yourself!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                            return false;
                        }
                    }
                }
                else if (GameInventoryObjectExtensions.IsHousingInventorySlot(toClientSlot))
                {
                    if (HasPermissionToMove(player))
                    {
                        if (GetClientInventory(player).TryGetValue((int)toClientSlot, out _))
                        {
                            player.Client.Out.SendMessage("You can only move an item to an empty slot!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                            return false;
                        }

                        GameInventoryObjectExtensions.NotifyObservers(this, player, _observers, GameInventoryObjectExtensions.MoveItem(this, player, fromClientSlot, toClientSlot, count));
                    }
                    else
                        return false;
                }
            }

            return true;
        }

        public virtual bool OnAddItem(GamePlayer player, DbInventoryItem item)
        {
            player.TempProperties.SetProperty(ITEM_BEING_ADDED, item);
            return MarketCache.AddItem(item);
        }

        public virtual bool OnRemoveItem(GamePlayer player, DbInventoryItem item)
        {
            item.OwnerLot = 0;
            item.SellPrice = 0;
            return MarketCache.RemoveItem(item);
        }

        public virtual bool SetSellPrice(GamePlayer player, eInventorySlot clientSlot, uint price)
        {
            if (player.ActiveInventoryObject is not EchoppeMerchant echoppeMerchant)
                return false;

            if (!HasPermissionToMove(player))
                return false;

            if (player.TempProperties.TryRemoveProperty(ITEM_BEING_ADDED, out object result))
            {
                if (result is not DbInventoryItem item)
                    return false;

                if (item.IsTradable)
                {
                    item.SellPrice = (int)price;
                    player.Out.SendMessage("Price set!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                }
                else
                {
                    item.SellPrice = 0;
                    player.Out.SendCustomDialog("This item is not tradable. You can store it here but cannot sell it.", null);
                }

                item.OwnerLot = 0;
                item.OwnerID = OwnerID;
                GameServer.Database.SaveObject(item);
            }

            return true;
        }

        public virtual bool SearchInventory(GamePlayer player, MarketSearch.SearchData searchData)
        {
            return false;
        }

        public virtual void OnPlayerBuy(GamePlayer player, eInventorySlot fromClientSlot, eInventorySlot toClientSlot, bool usingMarketExplorer = false)
        {
            Dictionary<int, DbInventoryItem> clientInventory = GetClientInventory(player);
            DbInventoryItem fromItem = null;

            if (clientInventory.TryGetValue((int)fromClientSlot, out DbInventoryItem value))
                fromItem = value;

            if (fromItem == null)
            {
                ChatUtil.SendErrorMessage(player, "I can't find the item you want to purchase!");
                return;
            }

            if (player.TargetObject == this)
            {
                player.TempProperties.SetProperty(CONSIGNMENT_BUY_ITEM, fromClientSlot);
                player.Out.SendCustomDialog($"Do you want to buy this item?", new CustomDialogResponse(BuyResponse));
            }
            else
            {
                ChatUtil.SendErrorMessage(player, "I'm sorry, you need to be talking to the merchant in order to make a purchase.");
            }
        }

        protected virtual void BuyResponse(GamePlayer player, byte response)
        {
            if (response != 0x01)
            {
                player.TempProperties.RemoveProperty(CONSIGNMENT_BUY_ITEM);
                return;
            }

            BuyItem(player);
        }

        protected virtual void BuyItem(GamePlayer player, bool usingMarketExplorer = false)
        {
            eInventorySlot fromClientSlot = player.TempProperties.GetProperty(CONSIGNMENT_BUY_ITEM, eInventorySlot.Invalid);
            player.TempProperties.RemoveProperty(CONSIGNMENT_BUY_ITEM);

            if (fromClientSlot != eInventorySlot.Invalid)
            {
                // Temporairement desactive comme dans l'original. 
                // A integrer plus tard avec MarketService
                player.Out.SendMessage("Le système d'achat est en cours d'adaptation.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
            }
        }

        public virtual void AddObserver(GamePlayer player)
        {
            _observers.TryAdd(player.Name, player);
        }

        public virtual void RemoveObserver(GamePlayer player)
        {
            _observers.Remove(player.Name);
        }

        public override bool Interact(GamePlayer player)
        {
            if (!base.Interact(player))
                return false;

            CheckInventory();

            if (player.ActiveInventoryObject != null)
            {
                player.ActiveInventoryObject.RemoveObserver(player);
                player.ActiveInventoryObject = null;
            }

            player.ActiveInventoryObject = this;
            AddObserver(player);

            if (HasPermissionToMove(player))
            {
                player.Out.SendInventoryItemsUpdate(GetClientInventory(player), eInventoryWindowType.ConsignmentOwner);
                long amount = _money;
                player.Out.SendConsignmentMerchantMoney(amount);
            }
            else
            {
                player.Out.SendInventoryItemsUpdate(GetClientInventory(player), eInventoryWindowType.ConsignmentViewer);
            }

            return true;
        }

        public override bool AddToWorld()
        {
            if (string.IsNullOrEmpty(OwnerID))
                return false;

            DbHouseConsignmentMerchant houseCM = DOLDB<DbHouseConsignmentMerchant>.SelectObject(DB.Column("OwnerID").IsEqualTo(OwnerID).And(DB.Column("HouseNumber").IsEqualTo(0)));

            if (houseCM == null)
            {
                houseCM = new DbHouseConsignmentMerchant
                {
                    OwnerID = OwnerID,
                    HouseNumber = 0,
                    Money = 0
                };
                GameServer.Database.AddObject(houseCM);
            }

            TotalMoney = houseCM.Money;
            
            // Apparence de l'échoppe
            Model = 1494; // Modèle de table de marché
            MaxSpeedBase = 0;

            base.AddToWorld();
            CheckInventory();

            // Démarrer le timer de despawn automatique (vérifie toutes les 3 secondes)
            _despawnTimer = new GameTimer(this.GetWeakReference(), 3000, new RegionTimerCallback(CheckOwnerState));

            return true;
        }

        protected virtual int CheckOwnerState(RegionTimer timer)
        {
            if (PlayerOwner == null || PlayerOwner.ObjectState != GameObject.eObjectState.Active || PlayerOwner.Client == null)
            {
                // Joueur déconnecté ou invalide
                this.Delete();
                return 0; // stop
            }

            if (!this.IsWithinRadius(PlayerOwner, 2000))
            {
                // Joueur trop éloigné
                PlayerOwner.TempProperties.RemoveProperty("ActiveEchoppe");
                PlayerOwner.Out.SendMessage("Votre échoppe s'est fermée car vous vous en êtes trop éloigné.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                this.Delete();
                return 0; // stop
            }

            return 3000; // relancer dans 3 secondes
        }

        public override void Delete()
        {
            if (_despawnTimer != null)
            {
                _despawnTimer.Stop();
                _despawnTimer = null;
            }
            base.Delete();
        }

        public virtual bool CheckInventory()
        {
            if (string.IsNullOrEmpty(OwnerID))
                return false;

            bool isFixed = false;
            // On s'assure que les items de ce proprio avec HouseNumber = 0 sont bien dans le MarketCache
            IList<DbInventoryItem> items = DOLDB<DbInventoryItem>.SelectObjects(DB.Column("OwnerID").IsEqualTo(OwnerID).And(DB.Column("SlotPosition").IsGreaterOrEqualTo(FirstDbSlot)).And(DB.Column("SlotPosition").IsLessOrEqualTo(LastDbSlot)).And(DB.Column("OwnerLot").IsEqualTo(0)));

            foreach (DbInventoryItem item in items)
            {
                if (!MarketCache.Items.Contains(item))
                {
                    MarketCache.AddItem(item);
                    isFixed = true;
                }
            }

            return isFixed;
        }
    }
}
