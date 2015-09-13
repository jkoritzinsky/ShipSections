/* Part of KSPPluginFramework
Version 1.2

Forum Thread:http://forum.kerbalspaceprogram.com/threads/66503-KSP-Plugin-Framework
Author: TriggerAu, 2014
License: The MIT License (MIT)

Modified by Jeremy Koritzinsky, 2015 for ShipSections
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using System.IO;

namespace KSPPluginFramework
{
    public abstract class ConfigNodeStorage : IPersistenceLoad, IPersistenceSave
    {
        private string _FilePath;
        /// <summary>
        /// Class Constructor
        /// </summary>
        protected ConfigNodeStorage() { }
        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="FilePath">Set the path for saving and loading. This can be an absolute path (eg c:\test.cfg) or a relative path from the location of the assembly dll (eg. ../config/test)</param>
        protected ConfigNodeStorage(string FilePath) { this.FilePath = FilePath; }

        /// <summary>
        /// Returns the current object as a ConfigNode
        /// </summary>
        public ConfigNode AsConfigNode
        {
            get
            {
                try
                {
                    //Create a new Empty Node with the class name
                    var cnTemp = new ConfigNode(GetType().Name);
                    //Load the current object in there
                    cnTemp = ConfigNode.CreateConfigFromObject(this, cnTemp);
                    return cnTemp;
                }
                catch (Exception ex)
                {
                    LogFormatted("Failed to generate ConfigNode-Error;{0}", ex.Message);
                    //Logging and return value?
                    return new ConfigNode(GetType().Name);
                }
            }
        }

        /// <summary>
        /// Test whether the configured FilePath exists
        /// </summary>
        /// <returns>True if its there</returns>
        public Boolean FileExists
        {
            get
            {
                return System.IO.File.Exists(FilePath);
            }
        }

        /// <summary>
        /// Gets the filename portion of the FullPath
        /// </summary>
        public String FileName
        {
            get { return Path.GetFileName(FilePath); }
        }
        /// <summary>
        /// Location of file for saving and loading methods
        /// 
        /// This can be an absolute path (eg c:\test.cfg) or a relative path from the location of the assembly dll (eg. ../config/test)
        /// </summary>
        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                //Combine the Location of the assembly and the provided string. This means we can use relative or absolute paths
                _FilePath = Path.Combine(Path.Combine(HighLogic.SaveFolder, GetSaveFolderFromGameState()), value).Replace(@"\", "/");
            }
        }

        private static string GetSaveFolderFromGameState()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                return Path.Combine("Ships", ShipConstruction.ShipType.ToString());
            }
            return Path.Combine("Ships", "InFlight");
        }

        /// <summary>
        /// Loads the object from the ConfigNode structure in the previously supplied file
        /// </summary>
        /// <returns>Succes of Load</returns>
        public bool Load() => Load(FilePath);

        /// <summary>
        /// Loads the object from the ConfigNode structure in a file
        /// </summary>
        /// <param name="fileFullName">Absolute Path to the file to load the ConfigNode structure from</param> 
        /// <returns>Success of Load</returns>
        public bool Load(string fileFullName)
        {
            var blnReturn = false;
            try
            {
                LogFormatted_DebugOnly("Loading ConfigNode");
                if (FileExists)
                {
                    //Load the file into a config node
                    var cnToLoad = ConfigNode.Load(fileFullName);
                    //remove the wrapper node that names the class stored
                    var cnUnwrapped = cnToLoad.GetNode(GetType().Name);
                    //plug it in to the object
                    ConfigNode.LoadObjectFromConfig(this, cnUnwrapped);
                    blnReturn = true;
                }
                else
                {
                    LogFormatted("File could not be found to load({0})", fileFullName);
                    blnReturn = false;
                }
            }
            catch (Exception ex)
            {
                LogFormatted("Failed to Load ConfigNode from file({0})-Error:{1}", fileFullName, ex.Message);
                LogFormatted("Storing old config - {0}", fileFullName + ".err-" + $"ddMMyyyy-HHmmss");
                System.IO.File.Copy(fileFullName, fileFullName + ".err-" + $"ddMMyyyy-HHmmss", true);
                blnReturn = false;
            }
            return blnReturn;
        }

        /// <summary>
        /// This overridable function executes whenever the object is loaded from a config node structure. Use this for complex classes that need decoding from simple confignode values
        /// </summary>
        public virtual void OnDecodeFromConfigNode() { }
        /// <summary>
        /// This overridable function executes whenever the object is encoded to a config node structure. Use this for complex classes that need encoding into simple confignode values
        /// </summary>
        public virtual void OnEncodeToConfigNode() { }

        /// <summary>
        /// Saves the object to a ConfigNode structure in the previously supplied file
        /// </summary>
        /// <returns>Succes of Save</returns>
        public bool Save() => Save(FilePath);

        /// <summary>
        /// Saves the object to a ConfigNode structure in a file
        /// </summary>
        /// <param name="fileFullName">Absolute Path to the file to load the ConfigNode structure from</param> 
        /// <returns>Success of Save</returns>
        public bool Save(string fileFullName)
        {
            LogFormatted_DebugOnly("Saving ConfigNode");
            var blnReturn = false;
            try
            {
                //Encode the current object
                var cnToSave = AsConfigNode;
                //Wrap it in a node with a name of the class
                var cnSaveWrapper = new ConfigNode(GetType().Name);
                cnSaveWrapper.AddNode(cnToSave);
                //Save it to the file
                cnSaveWrapper.Save(fileFullName);
                blnReturn = true;
            }
            catch (Exception ex)
            {
                LogFormatted("Failed to Save ConfigNode to file({0})-Error:{1}", fileFullName, ex.Message);
                blnReturn = false;
            }
            return blnReturn;
        }

        /// <summary>
        /// Some Structured logging to the debug file
        /// </summary>
        /// <param name="Message">Text to be printed - can be formatted as per String.format</param>
        /// <param name="strParams">Objects to feed into a String.format</param>
        internal static void LogFormatted(String Message, params object[] strParams)
        {
            Message = string.Format(Message, strParams);                  // This fills the params into the message
            var strMessageLine = $"{DateTime.Now},{Message}";                                           // This adds our standardised wrapper to each line
            UnityEngine.Debug.Log(strMessageLine);                        // And this puts it in the log
        }
        
        /// <summary>
        /// Some Structured logging to the debug file - ONLY RUNS WHEN DLL COMPILED IN DEBUG MODE
        /// </summary>
        /// <param name="Message">Text to be printed - can be formatted as per String.format</param>
        /// <param name="strParams">Objects to feed into a String.format</param>
        [System.Diagnostics.Conditional("DEBUG")]
        internal static void LogFormatted_DebugOnly(String Message, params object[] strParams)
        {
            LogFormatted("DEBUG: " + Message, strParams);
        }

        /// <summary>
        /// Wrapper for our overridable functions
        /// </summary>
        void IPersistenceLoad.PersistenceLoad()
        {
            OnDecodeFromConfigNode();
        }
        /// <summary>
        /// Wrapper for our overridable functions
        /// </summary>
        void IPersistenceSave.PersistenceSave()
        {
            OnEncodeToConfigNode();
        }

    }
}