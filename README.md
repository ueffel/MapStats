MapStats
========

Kleines Analysetool um ein paar Statistiken über die Mapverteilung in World of
Tanks zu generieren. Funktioniert anhand der Replays. Das heißt einfach in den
Replayordner legen und ausführen.

## Compilen

```
$ csc -utf8output -o mapstats.cs
```
oder falls csc nicht im PATH
```
$ c:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -utf8output -o mapstats.cs
```

## Benutzung

Siehe
```
$ mapstats --help
```

## Beispielausgabe
```
Stats für: alles
Gefechte: 3309
erwartete Gefechte pro Map: 80,71
Ruinberg             168 (5,08%)
Prohorovka           167 (5,05%)
Himmelsdorf          159 (4,81%)
Lakeville            159 (4,81%)
Steppen              154 (4,65%)
Minen                149 (4,50%)
Malinovka            136 (4,11%)
Ensk                 135 (4,08%)
Murowanka            128 (3,87%)
Klippen              117 (3,54%)
Kloster              115 (3,48%)
El Halluf            112 (3,38%)
Heiliges Tal         110 (3,32%)
Redshire             107 (3,23%)
Komarin               83 (2,51%)
Bergpass              83 (2,51%)
Wadi                  80 (2,42%)
Südküste              79 (2,39%)
Erlenberg             78 (2,36%)
Highway               72 (2,18%)
Severogorsk           71 (2,15%)
Küste                 69 (2,09%)
Tundra                68 (2,06%)
Live Oaks             65 (1,96%)
Polargebiet           65 (1,96%)
Siegfriedlinie        64 (1,93%)
Fjorde                64 (1,93%)
Fischerbucht          63 (1,90%)
Westfield             62 (1,87%)
Flugplatz             60 (1,81%)
Moor                  51 (1,54%)
Perlenfluss           50 (1,51%)
Karelien              43 (1,30%)
Nordwesten            40 (1,21%)
Hafen                 36 (1,09%)
Verstecktes Dorf      15 (0,45%)
Provinz               14 (0,42%)
Weitpark               7 (0,21%)
Ruinberg in Flammen    5 (0,15%)
Windsturm              4 (0,12%)
Himmelsdorf Winter     2 (0,06%)
----------
```
