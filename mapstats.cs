using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class replaystats
{
	/// <summary>
	/// Hashtable that Maps all the numbers in replay file names to map names
	/// </summary>
	public static Hashtable maps = new Hashtable()
	{
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
		{"92", "Stalingrad"},
		{"95", "Geisterstadt"},
		{"96", "Feuriger Bogen"},
		{"100", "Mittengard"},
		{"101", "Overlord"},
		{"103", "Winterberg"}
	};

	/// <summary>
	/// Helper function to do the formated output
	/// </summary>
	///
	/// <param name="dict">Dictionary&lt;string, int&gt; with
	/// (Key, Value) = (map name, number of battles)</param>
	/// <param name="sum">int sum of all battles in the dict.</param>
	public static void printOut(Dictionary<string, int> dict, int sum)
	{
		foreach (KeyValuePair<string, int> x in dict) {
			Console.WriteLine("{0,-23} {1,3} ({2,4:#0.00}%)",
				maps[x.Key] == null ? x.Key : maps[x.Key],
				x.Value,
				(double) x.Value / sum * 100.0
			);
		}
	}

	public static void Main(string[] args)
	{
		List<string> fileslist = new List<string>();
		List<string> paths     = new List<string>();
		List<int>    groups    = new List<int>();
		List<string> patterns  = new List<string>(){"*"};

		// process commandline parameter
		for(int i = 0; i < args.Length; i++)
		{
			// take care of the pattens parameter
			switch (args[i])
			{
				case "-p":
				case "--pattern":
					patterns.Add(args[i+1]);
					i++;
					break;
				// take care of groups
				case "-g":
				case "--group":
					switch (args[i+1])
					{
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
		if (patterns.Count > 1)
		{
			patterns.Remove("*");
		}

		// add all files from the path
		foreach(string arg in paths)
		{
			foreach(string pattern in patterns)
			{
				if (string.IsNullOrEmpty(arg))
				{
					continue;
				}
				try
				{
					fileslist.AddRange(Directory.GetFiles(arg, pattern));
				}
				catch (System.IO.DirectoryNotFoundException e)
				{
					Console.WriteLine("Fehler: {0}", e.Message);
				}
			}
		}

		// if only a pattern was provided but no path, apply pattern to cwd
		if (fileslist.Count == 0)
		{
			foreach(string pattern in patterns)
			{
				fileslist.AddRange(Directory.GetFiles("./", pattern));
			}
		}

		// remove duplicates
		fileslist = fileslist.Distinct().ToList();

		// parse the filenames and group them
		Regex exp = new Regex(@"(\d+)_\d+_(.+?)-(.+)_(\d+)_(.+).wotreplay");
		List<string[]> battles = fileslist.Where(file => exp.IsMatch(file))
			.Select(file => exp.Match(file).Groups.Cast<Group>().Skip(1).Take(4).Select(g => g.Value).ToArray())
			.ToList();

		Dictionary<string, Dictionary<string, int>> stats = battles
			.GroupBy(battle => (groups.Count == 0 ? "alles" : "")
					+ string.Join(" ", battle.Where((battleProps, index) => groups.Contains(index))))
			.Select(battleGroup => new {
				key = battleGroup.Key,
				val = battleGroup.GroupBy(battle => battle[3])
					.OrderByDescending(battle => battle.Count())
					.ThenBy(battle => battle.Key)
					.ToDictionary(x => x.Key, x => x.Count()) })
			.OrderBy(x => x.key)
			.ToDictionary(x => x.key, x => x.val);

		// print that shit
		foreach(KeyValuePair<string, Dictionary<string, int>> dict in stats)
		{
			int sum = dict.Value.Sum(x => x.Value);
			Console.WriteLine("Stats für: {0}", dict.Key);
			Console.WriteLine("Gefechte: {0,4}\r\nerwartete Gefechte pro Map: {1,3:###0.00}",
				sum,
				sum / (maps.Count - 2.0)
			);
			printOut(dict.Value, sum);
			Console.WriteLine("----------");
		}

		// stahp
		Console.ReadKey();
	}
}
