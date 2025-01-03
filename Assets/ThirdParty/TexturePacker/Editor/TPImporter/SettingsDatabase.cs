using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace TPImporter
{
	[InitializeOnLoad]
	public class SettingsDatabase : ScriptableObject
	{
		private int version = 2;

		private bool importPivots = true;

		private List<string> tpsheetFileNames = new List<string>();

		private List<string> textureFileNames = new List<string>();

		private List<string> normalmapFileNames = new List<string>();

		private const int DATABASE_VERSION = 2;

		private const string PIVOTPOINT_KEY = "TPImporter.ImportPivotPoints";

		private const string IMPORTER_DLL = "TexturePackerImporter.dll";

		private const string DATABASE_FILE = "SettingsTexturePackerImporter.txt";

		private const string ASSET_FILE = "SettingsTexturePackerImporter.asset";

		private const string DEFAULT_PATH = "/codeandweb.com/Editor";

		private const string EXAMPLE_SHEET = "/codeandweb.com/Example/SpriteSheet/sprites.tpsheet";

		private static string databaseFilePath;

		private static string assetFilePath;

		private static SettingsDatabase instance;

		[CompilerGenerated]
		private static EditorApplication.CallbackFunction callback;

		static SettingsDatabase()
		{
			SettingsDatabase.instance = null;
			Dbg.Log("Adding delayed call");
			Delegate arg_32_0 = EditorApplication.delayCall;
			if (SettingsDatabase.callback == null)
			{
				SettingsDatabase.callback = new EditorApplication.CallbackFunction(SettingsDatabase.InitDatabase);
			}
			EditorApplication.delayCall = (EditorApplication.CallbackFunction)Delegate.Combine(arg_32_0, SettingsDatabase.callback);
		}

		private static void InitDatabase()
		{
			Dbg.Log("Initializing...");
			SettingsDatabase settingsDatabase = SettingsDatabase.getInstance();
			Dbg.Log("TexturePacker Importer initialized");
			if (File.Exists(SettingsDatabase.fixPath(Application.dataPath + "/codeandweb.com/Example/SpriteSheet/sprites.tpsheet")) && !settingsDatabase.containsDataFile(SettingsDatabase.fixPath("Assets/codeandweb.com/Example/SpriteSheet/sprites.tpsheet")))
			{
				Dbg.Log("Initializing example sheet");
				AssetDatabase.ImportAsset(SettingsDatabase.fixPath("Assets/codeandweb.com/Example/SpriteSheet/sprites.tpsheet"), ImportAssetOptions.ForceUpdate);
			}
		}

		public void Awake()
		{
			Dbg.Log("Awake " + base.GetInstanceID());
		}

		public void OnDestroy()
		{
			Dbg.Log("OnDestroy " + base.GetInstanceID());
			if (this == SettingsDatabase.instance)
			{
				this.saveDatabase();
				SettingsDatabase.instance = null;
				Dbg.Log("settings database destroyed");
			}
			else
			{
				Dbg.Log("destructor of partially initialized database object called; ignoring");
			}
		}

		public void OnDisable()
		{
			Dbg.Log("OnDisable " + base.GetInstanceID());
		}

		public void OnEnable()
		{
			Dbg.Log("OnEnable " + base.GetInstanceID());
		}

		public void addSheet(string dataFile, string textureFile, string normalmapFile)
		{
			this.removeSheet(dataFile, false);
			Dbg.Log("add sheet");
			this.tpsheetFileNames.Add(dataFile);
			this.textureFileNames.Add(textureFile);
			this.normalmapFileNames.Add(normalmapFile);
			this.saveDatabase();
		}

		public void removeSheet(string dataFile, bool save = true)
		{
			int num = this.tpsheetFileNames.IndexOf(dataFile);
			if (num >= 0)
			{
				this.tpsheetFileNames.RemoveAt(num);
				this.textureFileNames.RemoveAt(num);
				this.normalmapFileNames.RemoveAt(num);
				if (save)
				{
					Dbg.Log("remove sheet");
					this.saveDatabase();
				}
			}
		}

		public bool containsDataFile(string dataFile)
		{
			int num = this.tpsheetFileNames.IndexOf(dataFile);
			return num >= 0;
		}

		public string spriteFileForNormalsFile(string normalsFile)
		{
			int num = this.normalmapFileNames.IndexOf(normalsFile);
			return (num < 0) ? null : this.textureFileNames[num];
		}

		public string spriteFileForDataFile(string dataFile)
		{
			int num = this.tpsheetFileNames.IndexOf(dataFile);
			return (num < 0) ? null : this.textureFileNames[num];
		}

		public string normalsFileForDataFile(string dataFile)
		{
			int num = this.tpsheetFileNames.IndexOf(dataFile);
			return (num < 0) ? null : this.normalmapFileNames[num];
		}

		public string dataFileForSpriteFile(string spriteFile)
		{
			int num = this.textureFileNames.IndexOf(spriteFile);
			return (num < 0) ? null : this.tpsheetFileNames[num];
		}

		public bool isSpriteSheet(string textureFile)
		{
			return this.textureFileNames.Contains(textureFile);
		}

		public bool isNormalmapSheet(string textureFile)
		{
			return this.normalmapFileNames.Contains(textureFile);
		}

		public List<string> allDataFiles()
		{
			return new List<string>(this.tpsheetFileNames);
		}

		public bool importPivotPoints()
		{
			return this.importPivots;
		}

		private static string fixPath(string path)
		{
			return path.Replace("\\", "/").Replace("//", "/");
		}

		private static void updateFileLocations()
		{
			Dbg.Log("updateFileLocations()");
			string dataPath = Application.dataPath;
			string text = dataPath + "/codeandweb.com/Editor";
			if (!File.Exists(text + "/TexturePackerImporter.dll"))
			{
				string[] files = Directory.GetDirectories(dataPath, "TPImporter", SearchOption.AllDirectories);
				if (files.Length > 0)
				{
					text = files[0].Remove(files[0].Length - "TPImporter".Length);
				}
				else
				{
					Dbg.Log("TexturePackerImporter.dll not found in " + dataPath);
				}
			}
			string str = "Assets/" + text.Remove(0, dataPath.Length);
			SettingsDatabase.databaseFilePath = SettingsDatabase.fixPath(text + "/SettingsTexturePackerImporter.txt");
			SettingsDatabase.assetFilePath = SettingsDatabase.fixPath(str + "/SettingsTexturePackerImporter.asset");
			Dbg.Log("database location: " + SettingsDatabase.databaseFilePath + "\nasset location: " + SettingsDatabase.assetFilePath);
		}

		public static SettingsDatabase getInstance()
		{
			Dbg.Log("getInstance()");
			if (SettingsDatabase.instance == null)
			{
				SettingsDatabase.updateFileLocations();
				SettingsDatabase.instance = ScriptableObject.CreateInstance<SettingsDatabase>();
				SettingsDatabase.instance.loadDatabase();
			}
			Dbg.Log(string.Concat(new object[]
			{
				"instance id: ",
				SettingsDatabase.instance.GetInstanceID(),
				", ",
				SettingsDatabase.instance.tpsheetFileNames.Count,
				" sheets"
			}));
			return SettingsDatabase.instance;
		}

		private static List<string> readStringList(StreamReader file)
		{
			List<string> list = new List<string>();
			while (file.Peek() == 45)
			{
				string item = file.ReadLine().Remove(0, 1).Trim();
				list.Add(item);
			}
			return list;
		}

		private void loadDatabase()
		{
			Dbg.Log("Loading database, instance id: " + base.GetInstanceID());
			if (SettingsDatabase.databaseFilePath == null)
			{
				Dbg.Log("databaseFilePath is null");
				return;
			}
			try
			{
				StreamReader streamReader = new StreamReader(SettingsDatabase.databaseFilePath);
				this.importPivots = EditorPrefs.GetBool("TPImporter.ImportPivotPoints", true);
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					string[] array = text.Split(new char[]
					{
						':'
					});
					string a = array[0].Trim();
					string s = array[1].Trim();
					if (a == "version")
					{
						this.version = int.Parse(s);
					}
					else if (a == "importPivots")
					{
						this.importPivots = (int.Parse(s) != 0);
					}
					else if (a == "tpsheetFileNames")
					{
						this.tpsheetFileNames = SettingsDatabase.readStringList(streamReader);
					}
					else if (a == "textureFileNames")
					{
						this.textureFileNames = SettingsDatabase.readStringList(streamReader);
					}
					else if (a == "normalmapFileNames")
					{
						this.normalmapFileNames = SettingsDatabase.readStringList(streamReader);
					}
					else if (a == "enableDebugOutput")
					{
						Dbg.enabled = (int.Parse(s) != 0);
					}
				}
				streamReader.Close();
			}
			catch (IOException)
			{
				Dbg.Log("exception catched");
			}
			Dbg.Log(string.Concat(new object[]
			{
				"Loaded database ",
				SettingsDatabase.databaseFilePath,
				" (",
				this.tpsheetFileNames.Count,
				" sheets)"
			}));
		}

		private static void swap<T>(List<T> list, int index1, int index2)
		{
			T value = list[index1];
			list[index1] = list[index2];
			list[index2] = value;
		}

		private void sortLists()
		{
			for (int i = 0; i < this.tpsheetFileNames.Count; i++)
			{
				for (int j = i + 1; j < this.tpsheetFileNames.Count; j++)
				{
					if (string.Compare(this.tpsheetFileNames[i], this.tpsheetFileNames[j], true) > 0)
					{
						SettingsDatabase.swap<string>(this.tpsheetFileNames, i, j);
						SettingsDatabase.swap<string>(this.textureFileNames, i, j);
						SettingsDatabase.swap<string>(this.normalmapFileNames, i, j);
					}
				}
			}
		}

		private static void writeStringList(StreamWriter file, List<string> list)
		{
			foreach (string current in list)
			{
				file.WriteLine("- " + current);
			}
		}

		private void saveDatabase()
		{
			Dbg.Log("Saving database, instance id: " + base.GetInstanceID());
			if (!File.Exists(SettingsDatabase.databaseFilePath))
			{
				SettingsDatabase.updateFileLocations();
			}
			if (SettingsDatabase.databaseFilePath == null)
			{
				Dbg.Log("databaseFilePath is null");
				return;
			}
			Dbg.Log(string.Concat(new object[]
			{
				"Saving database ",
				SettingsDatabase.databaseFilePath,
				" (",
				this.tpsheetFileNames.Count,
				" sheets)"
			}));
			StreamWriter streamWriter = new StreamWriter(SettingsDatabase.databaseFilePath);
			this.sortLists();
			streamWriter.WriteLine(string.Format("version: {0}", this.version));
			streamWriter.WriteLine(string.Format("importPivots: {0}", (!this.importPivots) ? 0 : 1));
			streamWriter.WriteLine("tpsheetFileNames:");
			SettingsDatabase.writeStringList(streamWriter, this.tpsheetFileNames);
			streamWriter.WriteLine("textureFileNames:");
			SettingsDatabase.writeStringList(streamWriter, this.textureFileNames);
			streamWriter.WriteLine("normalmapFileNames:");
			SettingsDatabase.writeStringList(streamWriter, this.normalmapFileNames);
			streamWriter.WriteLine(string.Format("enableDebugOutput: {0}", (!Dbg.enabled) ? 0 : 1));
			streamWriter.Close();
		}
	}
}
