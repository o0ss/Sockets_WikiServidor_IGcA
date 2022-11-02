import wikipedia
import sys

file = open("wiki_out.tmp", "wb")

query = ""
for tok in sys.argv[1:len(sys.argv)]:
    query += " " + tok

wikipedia.set_lang("en")

try:
    srch_results = wikipedia.search(query)
    summry = wikipedia.summary(srch_results[0], sentences=5)
    file.write(summry.encode(encoding='utf-8', errors='replace'))
    print("Resultado:\n" + summry + "\n")
except:
    file.write("Error. Intenta con otro término.".encode())
    print("\n\nException.\n")


file.close()