using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatTesting : MonoBehaviour
{
    public LLM m_ChatModel;
    
    public string userText = "";
    private string stackText = "";

    // Public lists accessible by outside functions
    public List<string> flavours = new List<string>();
    public List<string> textures = new List<string>();
    public List<string> preparations = new List<string>();
    public List<string> ingredients = new List<string>();

    void OnGUI()
    {
        userText = GUI.TextField(new Rect(10, Screen.height - 35, 275, 30), userText);

        if (GUI.Button(new Rect(10 + 275 + 10, Screen.height - 35, 75, 30), "SEND\n"))
        {
            SendData(userText);
            stackText = $"> {userText}\n{stackText}";
            userText = "";
        }

        GUI.Label(new Rect(10, Screen.height - 385, 350, 350), stackText);
    }

    public void SendData(string _postWord)
    {
        if (_postWord.Equals(""))
            return;
        
        Debug.Log("Sent message: " + _postWord);
        
        string _msg = $"I am brainstorming innovative and modern dessert concepts inspired by: '{_postWord}'. " +
                      $"Provide exactly three unique and creative keywords for each of these four categories to help invent a completely new dessert: Flavours, Textures, Preparation, and Ingredients. " +
                      $"You MUST respond strictly in the following format, with no markdown, conversational text, or bullet points:\n" +
                      $"Flavours: word1, word2, word3\n" +
                      $"Textures: word1, word2, word3\n" +
                      $"Preparation: word1, word2, word3\n" +
                      $"Ingredients: word1, word2, word3";

        m_ChatModel.PostMsg(_msg, CallBack);
    }

    private void CallBack(string _response)
    {
        // Clean the raw string
        _response = _response.Trim();
        _response = _response.Replace("*", ""); 

        Debug.Log("Response from AI:\n" + _response);
        stackText = $"Gemini>\n{_response}\n{stackText}";

        // Clear previous lists before storing new response data
        flavours.Clear();
        textures.Clear();
        preparations.Clear();
        ingredients.Clear();

        string[] lines = _response.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string[] categoryParts = line.Split(':');
            
            if (categoryParts.Length == 2)
            {
                string categoryName = categoryParts[0].Trim();
                string keywordString = categoryParts[1];

                Debug.Log($"--- Parsed Category: {categoryName} ---");

                string[] keywords = keywordString.Split(',');
                List<string> cleanedKeywords = new List<string>();

                foreach (string keyword in keywords)
                {
                    string cleanKeyword = keyword.Trim();
                    if (!string.IsNullOrEmpty(cleanKeyword))
                    {
                        cleanedKeywords.Add(cleanKeyword);
                        Debug.Log($"Extracted Node Idea ({categoryName}): {cleanKeyword}");
                    }
                }

                // Assign keywords to the respective public list based on category name
                switch (categoryName.ToLower())
                {
                    case "flavours":
                        flavours = cleanedKeywords;
                        break;
                    case "textures":
                        textures = cleanedKeywords;
                        break;
                    case "preparation":
                        preparations = cleanedKeywords;
                        break;
                    case "ingredients":
                        ingredients = cleanedKeywords;
                        break;
                }
            }
        }
    }
}