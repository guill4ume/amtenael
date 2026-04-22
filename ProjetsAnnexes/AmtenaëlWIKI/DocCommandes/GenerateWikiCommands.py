import os
import re
import sys

# Paths
CORE_PATH = r"C:\OpenDAOC_server\OpenDAoC-Core-master"
GAMESERVER_PATH = os.path.join(CORE_PATH, "GameServer")
COMMANDS_PATH = os.path.join(GAMESERVER_PATH, "commands")
SCRIPTS_PATH = os.path.join(GAMESERVER_PATH, "scripts")
LANG_PATH = os.path.join(GAMESERVER_PATH, r"language\FR")

# Privilege Levels
PRIV_LEVELS = {
    "1": "Joueur",
    "2": "GM",
    "3": "Admin",
    "ePrivLevel.Player": "Joueur",
    "ePrivLevel.GM": "GM",
    "ePrivLevel.Admin": "Admin"
}

# Translation Dictionary
translations = {}

def load_translations():
    template = re.compile(r'^([^:#\s]+)\s*:\s*(.*)$')
    for root, dirs, files in os.walk(LANG_PATH):
        for file in files:
            if file.endswith(".txt"):
                file_path = os.path.join(root, file)
                try:
                    with open(file_path, 'r', encoding='utf-8') as f:
                        for line in f:
                            line = line.strip()
                            if not line or line.startswith("#"):
                                continue
                            match = template.match(line)
                            if match:
                                key, val = match.groups()
                                translations[key.strip()] = val.strip()
                except Exception as e:
                    print(f"Error loading {file_path}: {e}")

def resolve(text):
    if not text:
        return ""
    text = text.strip().strip('"')
    if text in translations:
        return translations[text]
    return text

def parse_cs_file(file_path):
    commands = []
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
            
        matches = re.finditer(r'\[Cmd(?:Attribute)?\s*\((.*?)\)\]', content, re.DOTALL)
        for match in matches:
            args_str = match.group(1).strip()
            parts = []
            current = ""
            in_quotes = False
            bracket_level = 0
            for char in args_str:
                if char == '"':
                    in_quotes = not in_quotes
                elif char == '{' or char == '[':
                    bracket_level += 1
                elif char == '}' or char == ']':
                    bracket_level -= 1
                
                if char == ',' and not in_quotes and bracket_level == 0:
                    parts.append(current.strip())
                    current = ""
                else:
                    current += char
            parts.append(current.strip())
            
            if not parts:
                continue
                
            cmd_name = parts[0].strip('"').replace("&", "/")
            
            idx = 1
            aliases = []
            if idx < len(parts) and (parts[idx].startswith("new string[]") or parts[idx].startswith("{") or "new[]" in parts[idx]):
                alias_match = re.search(r'{(.*?)}', parts[idx])
                if alias_match:
                    aliases = [a.strip().strip('"').replace("&", "/") for a in alias_match.group(1).split(",") if a.strip()]
                idx += 1
            
            header = None
            if idx < len(parts) and parts[idx].startswith('"') and not ("ePrivLevel" in parts[idx] or parts[idx].strip().isdigit()):
                header = parts[idx].strip('"')
                idx += 1
                
            level = "Unknown"
            if idx < len(parts):
                level_raw = parts[idx].strip()
                level = PRIV_LEVELS.get(level_raw, level_raw)
                idx += 1
                
            desc = ""
            if idx < len(parts):
                desc = resolve(parts[idx])
                idx += 1
                
            usages = []
            while idx < len(parts):
                usages.append(resolve(parts[idx]))
                idx += 1
                
            commands.append({
                "name": cmd_name,
                "aliases": aliases,
                "level": level,
                "description": desc,
                "usages": usages
            })
    except Exception as e:
        print(f"Error parsing {file_path}: {e}")
    return commands

def main():
    load_translations()
    all_commands = []
    
    for folder in [COMMANDS_PATH, SCRIPTS_PATH]:
        if not os.path.exists(folder):
            continue
        for root, dirs, files in os.walk(folder):
            for file in files:
                if file.endswith(".cs"):
                    all_commands.extend(parse_cs_file(os.path.join(root, file)))
                    
    grouped = {}
    for cmd in all_commands:
        lvl = cmd["level"]
        if lvl not in grouped:
            grouped[lvl] = []
        grouped[lvl].append(cmd)
        
    level_map = {
        "Joueur": "Commandes_Joueur.txt",
        "GM": "Commandes_GM.txt",
        "Admin": "Commandes_Admin.txt"
    }
    
    for lvl, filename in level_map.items():
        if lvl not in grouped:
            print(f"No commands found for level {lvl}")
            continue
            
        output = f"====== Commandes {lvl} ======\n\n"
        output += "^ Commande ^ Alias ^ Description ^ Utilisation ^\n"
        
        cmds = sorted(grouped[lvl], key=lambda x: x["name"].lower())
        
        # Remove duplicates (sometimes commands are defined multiple times or across files)
        seen = set()
        unique_cmds = []
        for c in cmds:
            if c["name"] not in seen:
                unique_cmds.append(c)
                seen.add(c["name"])

        for cmd in unique_cmds:
            alias_str = ", ".join(cmd["aliases"]) if cmd["aliases"] else ""
            usage_str = " \\\\ ".join(cmd["usages"]) if cmd["usages"] else ""
            output += f"| **{cmd['name']}** | {alias_str} | {cmd['description']} | {usage_str} |\n"
        
        with open(filename, "w", encoding="utf-8") as f:
            f.write(output)
        
        print(f"Generated {filename}")

if __name__ == "__main__":
    main()
