import re
import sys

def extract_merchant_data(dump_path, item_list_ids, output_sql):
    ids_set = set(item_list_ids)
    extracted_merchant_items = []
    item_template_ids = set()
    
    print(f"Reading {dump_path}...")
    with open(dump_path, 'r', encoding='utf-8', errors='ignore') as f:
        for line in f:
            if 'INSERT INTO `merchantitem`' in line:
                # Basic parsing for merchantitem lines
                # Format: ('ID', 'ItemListID', 'ItemTemplateID', ...)
                matches = re.findall(r"\('([^']*)', '([^']*)', '([^']*)'", line)
                for mid, ilid, itid in matches:
                    if ilid in ids_set:
                        extracted_merchant_items.append(line.strip())
                        item_template_ids.add(itid)
                        break
            
            if 'INSERT INTO `itemtemplate`' in line:
                # We will need a second pass or check against templates
                pass

    print(f"Found {len(extracted_merchant_items)} merchant item chunks.")
    print(f"Referencing {len(item_template_ids)} item templates.")
    
    # Second pass for item templates
    extracted_templates = []
    with open(dump_path, 'r', encoding='utf-8', errors='ignore') as f:
        for line in f:
            if 'INSERT INTO `itemtemplate`' in line:
                # Format: ('Id_nb', 'Name', ...)
                # Id_nb is the first field
                match = re.search(r"VALUES \('([^']*)'", line)
                if match:
                    id_nb = match.group(1)
                    if id_nb in item_template_ids:
                        extracted_templates.append(line.strip())

    print(f"Extracted {len(extracted_templates)} item templates.")
    
    with open(output_sql, 'w', encoding='utf-8') as out:
        out.write("-- Migration Merchant Data for Zone 51\n")
        out.write("SET FOREIGN_KEY_CHECKS=0;\n\n")
        
        out.write("-- Item Templates\n")
        for t in extracted_templates:
            out.write(t + "\n")
            
        out.write("\n-- Merchant Items\n")
        for mi in extracted_merchant_items:
            out.write(mi + "\n")
            
        out.write("\nSET FOREIGN_KEY_CHECKS=1;\n")

if __name__ == "__main__":
    ids = []
    with open('itemlistids_region51.txt', 'r') as f:
        for line in f:
            l = line.strip()
            if l and l != 'ItemsListTemplateID':
                ids.append(l)
    
    extract_merchant_data('amtedump04.sql', ids, 'migration_merchants_51.sql')
