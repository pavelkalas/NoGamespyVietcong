# 🐍🔥 Vietcong Multiplayer Fix (bez úpravy `hosts` souboru)

> ⚠️ GameSpy chcípnul a s tím i servery ve Vietcongu. Tohle ti to fixne.  
> ✨ Bez potřeby rootovat si systém a hrabat se v `C:\Windows\System32\drivers\etc\hosts`.

## 🧠 Co to vlastně dělá?

- Přesměruje komunikaci hry na nový funkční serverlist tím, že injectne hru a přepíše adresu masterserveru na tu funkční
- Nepotřebuješ žádnou `hosts` úpravu či instalovat qtracker a podobné blbosti
- Funguje to **out of the box** – stačí spustit a pařit

## 💾 Jak to nainstalovat?

1. Stáhni `NoGamespyVietcong` z [releases](https://github.com/pavelkalas/NoGamespyVietcong/releases/latest), vlož soubory do složky, nahraď je a můžeš hrát!
2. Možná bude potřeba ve hře párkrát *poklikat* na tlačítko **Obnovit**, protože při spuštění hry hra pošle fetch na starý master, než se to přepíše v paměti na ten funkční..
