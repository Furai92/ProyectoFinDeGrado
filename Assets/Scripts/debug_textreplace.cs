using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class debug_textreplace : MonoBehaviour
{
    public TextMeshProUGUI texttoedit;

    private void OnEnable()
    {
        HashSet<string> targets = new HashSet<string>();
        HashSet<string> keywordsFound = new HashSet<string>();
        targets.Add("bruh");
        targets.Add("amogus");

        char[] separators = { ' ', '.',','  };
        string[] splitted = texttoedit.text.Split(separators);

        for (int i = 0; i < splitted.Length; i++) 
        {
            if (targets.Contains(splitted[i])) 
            {
                if (!keywordsFound.Contains(splitted[i])) { keywordsFound.Add(splitted[i]); }

            }

        }
        foreach (string s in keywordsFound)
        {
            texttoedit.text = texttoedit.text.Replace(s, string.Format("<color=green>{0}</color>", s));
        }
        

    }

}
