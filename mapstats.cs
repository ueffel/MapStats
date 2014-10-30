using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class replaystats {
	/**
	 * Maps all the numbers in replay file names to map names
	 * @type {Hashtable}
	 */
	public static Hashtable maps = new Hashtable() {
		{"00", "Tutorial"},
		{"01", "Karelien"},
		{"02", "Malinovka"},
		{"03", "Provinz"},
		{"04", "Himmelsdorf"},
		{"05", "Prohorovka"},
		{"06", "Ensk"},
		{"07", "Lakeville"},
		{"08", "Ruinberg"},
		{"10", "Minen"},
		{"11", "Murowanka"},
		{"13", "Erlenberg"},
		{"14", "Siegfriedlinie"},
		{"15", "Komarin"},
		{"17", "Weitpark"},
		{"18", "Klippe"},
		{"19", "Kloster"},
		{"22", "Moor"},
		{"23", "Westfield"},
		{"28", "Wadi"},
		{"29", "El Halluf"},
		{"31", "Flugplatz"},
		{"33", "Fjorde"},
		{"34", "Redshire"},
		{"35", "Steppe"},
		{"36", "Fischerbucht"},
		{"37", "Bergpass"},
		{"38", "Polargebiet"},
		{"39", "Südküste"},
		{"42", "Hafen"},
		{"43", "Northwest"},
		{"44", "Live Oaks"},
		{"45", "Highway"},
		{"47", "Küste"},
		{"51", "Drachenkamm"},
		{"53", "Verstecktes Dorf"},
		{"59", "Grosse Mauer"},
		{"60", "Perlenfluss"},
		{"63", "Tundra"},
		{"73", "Heiliges Tal"},
		{"83", "Kharkov"},
		{"84", "Windsturm"},
		{"85", "Severogorsk"},
		{"86", "Himmelsdorf im Winter"},
		{"87", "Ruinberg in Flammen"},
		{"96", "Feuriger Bogen"}
	};

	/**
	 * Helper function to do the formated output
	 *
	 * @param dict Dictionary<string, int> with (Key, Value) = (map name, number
	 *     of battles)
	 * @param sum int sum of all battles in the dict.
	 *
	 */
	public static void printOut(Dictionary<string, int> dict, int sum) {
		foreach (KeyValuePair<string, int> x in dict) {
			Console.WriteLine("{0,-20} {1,3} ({2,4:#0.00}%)",
				maps[x.Key] == null ? x.Key : maps[x.Key],
				x.Value,
				(double) x.Value / sum * 100.0
			);
		}
	}

	/**
	 * Parse the list of replay filenames to a list of arrays of strings. Every
	 * array has 4 fields (time, nation, tank, map) that are extracted via
	 * regular expression.
	 *
	 * @param fileslist List<string> of replay names to parse
	 *
	 * @return List<string[]> of 4 fields (s.a) for each replay name that was
	 *     parsed
	 */
	public static List<string[]> readList(List<string> fileslist) {
		List<string[]> stats = new List<string[]>();
		Regex exp = new Regex(@"(\d+)_\d+_(.+?)-(.+)_(\d{2})_(.+).wotreplay");
		foreach(string item in fileslist) {
			if (!exp.IsMatch(item)) {
				continue;
			}
			Match match = exp.Match(item);
			string[] replay = new string[4];
			// time
			replay[0] = match.Groups[1].Value;
			// nation
			replay[1] = match.Groups[2].Value;
			// tank
			replay[2] = match.Groups[3].Value;
			// map
			replay[3] = match.Groups[4].Value;
			stats.Add(replay);
		}

		return stats;
	}

	/**
	 * Takes a list with an array of 4 string fields (time, nation, tank, map)
	 * and groups them into a Dictionary with the provided groups. First a key
	 * for every group is constructed and then for each group the battles are
	 * counted.
	 *
	 * @param battles List<string[]> battles to count
	 * @param groups List<int> of groups
	 *
	 * @return Dictionary<string, Dictionary<string, int>>
	 */
	public static Dictionary<string, Dictionary<string, int>> groupbattles(List<string[]> battles,
			List<int> groups) {
		Dictionary<string, Dictionary<string, int>> result
			= new Dictionary<string, Dictionary<string, int>>();
		string key = "";

		foreach(string[] battle in battles) {
			// construct a artificial grouping key for the dict
			key = groups.Count == 0 ? "alles" : "";
			foreach(int g in groups) {
				key += " " + battle[g];
			}

			if (result.ContainsKey(key)) {
				Dictionary<string, int> mapcount = result[key];
				if (mapcount.ContainsKey(battle[3])) {
					mapcount[battle[3]]++;
				}
				else {
					mapcount.Add(battle[3], 1);
				}
				result[key] = mapcount;
			}
			else {
				Dictionary<string, int> mapcount = new Dictionary<string, int>();
				mapcount.Add(battle[3], 1);
				result.Add(key, mapcount);
			}
		}

		return result;
	}

	public static void Main(string[] args) {
		List<string> fileslist = new List<string>();
		List<string> paths     = new List<string>();
		List<int>    groups    = new List<int>();
		List<string> patterns  = new List<string>(){"*"};

		// process commandline parameter
		for(int i = 0; i < args.Length; i++) {
			// take care of the pattens parameter
			switch (args[i]) {
			case "-p":
			case "--pattern":
				patterns.Add(args[i+1]);
				i++;
				break;
			// take care of groups
			case "-g":
			case "--group":
				switch (args[i+1]) {
				case "time":
					groups.Add(0);
					break;
				case "nation":
					groups.Add(1);
					break;
				case "tank":
					groups.Add(2);
					break;
				default:
					Console.WriteLine("'" + args[i+1] + "' als Gruppierungsattribut"
						+ " kenne ich nicht, versuch mal 'mapstats --help'");
					return;
				}
				i++;
				break;
			// display help message
			case "-h":
			case "--help":
				Console.WriteLine("Benutzung: mapstats [Optionen] [Ordnerpfade]");
				Console.WriteLine("Optionen:");
				Console.WriteLine(" -p, --pattern MUSTER");
				Console.WriteLine(" -g, --group   GRUPPIERUNGSATTRIBUT");
				Console.WriteLine("     --help    Zeigt diese Hilfe an");
				Console.WriteLine();
				Console.WriteLine("Muster:");
				Console.WriteLine("- sowas wie zum Beispiel \"20130814*\" ist"
					+ " für alle Replays des 14. August 2013\r\n"
					+ "  (Doppelanführungszeichen weil manche Shells das * selber"
					+ " interpretieren wollen)");
				Console.WriteLine("- \"*T-54*\" gibt die Mapstatistiken nur für"
					+ " den T-54 an.");
				Console.WriteLine();
				Console.WriteLine("Gruppierungsattribut:");
				Console.WriteLine("- kann 'nation', 'tank' oder 'time' sein."
					+ " (bei 'time' wird Tagweise gruppiert)");
				Console.WriteLine();
				Console.WriteLine("- \"erwartete Gefechte\" in der Ausgabe meint"
					+ " die Anzahl der Gefechte bei\r\n"
					+ "  Gleichverteilung der Maps ({0} Maps, ohne Provinz & "
					+ "Weitpark), also so etwa {1:0.00}%", maps.Count - 2, 100.0 / (maps.Count - 2));
				return;
			// no parameter, must be a path ;)
			default:
				paths.Add(args[i]);
				break;
			}
		}

		// if pattern was provided, remove default pattern '*'
		if (patterns.Count > 1) {
			patterns.Remove("*");
		}

		// add all files from the path
		foreach(string arg in paths) {
			foreach(string pattern in patterns) {
				if (string.IsNullOrEmpty(arg)) {
					continue;
				}
				try {
					fileslist.AddRange(Directory.GetFiles(arg, pattern));
				}
				catch (System.IO.DirectoryNotFoundException e) {
					Console.WriteLine("Fehler: {0}", e.Message);
				}
			}
		}

		// if only a pattern was provided but no path, apply pattern to cwd
		if (fileslist.Count == 0) {
			foreach(string pattern in patterns) {
				fileslist.AddRange(Directory.GetFiles("./", pattern));
			}
		}

		// remove duplicates
		fileslist = fileslist.Distinct().ToList();

		// parse the filenames and group them
		List <string[]> battles = readList(fileslist);
		Dictionary<string, Dictionary<string, int>> stats = groupbattles(battles, groups);

		// sort outer dict
		stats = stats.OrderBy(x => x.Key)
			.ToDictionary(x => x.Key, x => x.Value);

		// sort inner dict
		Dictionary<string, Dictionary<string, int>> statstemp
			= new Dictionary<string, Dictionary<string, int>>();
		Dictionary<string, int> sums = new Dictionary<string, int>();
		foreach(string key in stats.Keys) {
			statstemp.Add(key, stats[key].OrderByDescending(x => x.Value)
				.ToDictionary(x => x.Key, x => x.Value));
			int sum = 0;
			foreach(int count in stats[key].Values) {
				sum += count;
			}
			sums.Add(key, sum);
		}
		stats = statstemp;

		// print that shit
		foreach(KeyValuePair<string, Dictionary<string, int>> dict in stats) {
			Console.WriteLine("Stats für: {0}", dict.Key);
			Console.WriteLine("Gefechte: {0,4}\r\nerwartete Gefechte pro Map: {1,3:###0.00}",
				sums[dict.Key],
				sums[dict.Key] / (maps.Count - 2.0)
			);
			printOut(dict.Value, sums[dict.Key]);
			Console.WriteLine("----------");
		}

		// stahp
		Console.ReadKey();
	}
}
