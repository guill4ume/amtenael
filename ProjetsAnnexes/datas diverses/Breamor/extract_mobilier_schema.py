import os

file_path = r"C:\OpenDAOC_server\ProjetsAnnexes\Breamor\BDDamte030326.sql"
targets = ["CREATE TABLE `worldobject`", "CREATE TABLE `door`"]

with open(file_path, "r", encoding="utf-8", errors="ignore") as f:
    for line in f:
        for target in targets:
            if target in line:
                print(f"--- Found schema for {target} ---")
                print(line.strip())
                for _ in range(30):
                    l = f.readline()
                    if not l: break
                    print(l.strip())
                    if "CHARSET=" in l: break
                print("-" * 20)
