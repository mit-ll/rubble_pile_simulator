// DISTRIBUTION STATEMENT A. Approved for public release. Distribution is unlimited.
//  
// This material is based upon work supported by the Department of the Air Force under Air Force Contract No. FA8702-15-D-0001. Any opinions, findings, conclusions or recommendations expressed in this material are those of the author(s) and do not necessarily reflect the views of the Department of the Air Force.
//  
// © 2024 Massachusetts Institute of Technology.
// Subject to FAR52.227-11 Patent Rights - Ownership by the contractor (May 2014)
//  
// The software/firmware is provided to you on an As-Is basis
//  
// Delivered to the U.S. Government with Unlimited Rights, as defined in DFARS Part 252.227-7013 or 7014 (Feb 2014). Notwithstanding any copyright notice, U.S. Government rights in this work are defined by DFARS 252.227-7013 or DFARS 252.227-7014 as detailed above. Use of this work other than as specifically authorized by the U.S. Government may violate any copyrights that exist in this work.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CustomArgs : MonoBehaviour
{
    // Start is called before the first frame update
    private const string argumentPrefix = "-";

    [SerializeField]
    private TextAsset debugArguments = null;
    private static Dictionary<string,float> argDict = new Dictionary<string, float>();

    private string[] cliArgs;

    private static CustomArgs instance;

    public static CustomArgs Instance
    {
        get { return instance; }
    }
    
    //Special Cases
    private string ROSip;

    public string ROSIP
    {
        get
        {
            if (ROSip != null || ROSip == "")
            {
                return ROSip;
            }

            return "127.0.0.1";
        }
        set
        {
            ROSip = value;
        }
    }
    
    
    private void Awake()
    {
        //Singleton
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject); 
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance.gameObject);
        }
        
#if UNITY_EDITOR
        if (debugArguments != null)
        {
            ParseArguments(debugArguments.text.Split(new char[] {' ', '\n'}));
        }
#else
        cliArgs = Environment.GetCommandLineArgs();
        ParseArguments(cliArgs);
#endif
    }

    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        
    //Parser inspired by Warped Imagination, https://www.youtube.com/watch?v=np9oHlUeOFo
    private void ParseArguments(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(args[i]))
            {
                continue;
            }

            string key = args[i].Trim().ToLower();

            if (!key.StartsWith(argumentPrefix))
            {
                continue;
            }

            key = key.Substring(1, key.Length - 1);

            StringBuilder sb = new StringBuilder();
            
            for (int j = i + 1; j < args.Length; j++)
            {
                if (string.IsNullOrWhiteSpace(args[i]))
                {
                    continue;
                }

                string value = args[j].Trim().ToLower();

                if (value.StartsWith(argumentPrefix))
                {
                    break;
                }

                sb.Append(value);
                sb.Append(" ");

            }
            ParseArgument(key, sb.Length == 0 ? string.Empty : sb.ToString().Trim());
        }
    }

    private void ParseArgument(string key, string value)
    {
        //Special Cases
        //Catch IP address
        if (value.Contains("."))
        {
            ParseAsIP(value);
            return;
        }
        //If key exists from getwithdefault overwrite it
        if (argDict.ContainsKey(key))
        {
            argDict[key] = ParseValue(value);
            return;
        }
        argDict.Add(key,ParseValue(value));
    }

    private float ParseValue(string value)
    {
        //Convert string to float from arguments
        float.TryParse(value, out var floatValue);
        return floatValue;
    }

    private void ParseAsIP(string value)
    {
        ROSip = value;
        return;
    }
    
    public static float GetWithDefault(string key, float value)
    {
        //Deal with capital letters by removing them
        var _key = key.ToLower();
        //If there is an existing key return the value
        if (argDict.ContainsKey(_key))
        {
            return argDict[_key];
        }
        //Otherwise add key and set default value and return it
        argDict.Add(_key, value);
        
        return value;
    }
}
