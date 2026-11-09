using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatTesting : MonoBehaviour
{
    public LLM m_ChatModel;
    
    public string userText = "";
    private string stackText = "";

    void OnGUI()
    {
        userText = GUI.TextField(new Rect(10, Screen.height - 10 - 35, 275, 30), userText);

        if (GUI.Button(new Rect(10+275+10, Screen.height - 10 - 35, 75, 30), "SEND\n"))
        {
            SendData(userText);
            stackText = $"> {userText}\n{stackText}";
            userText = "";
        }

        GUI.Label(new Rect(10, Screen.height - 10 - 35 - 10 - 350, 350, 350), stackText);
    }

    public void SendData(string _postWord)
    {
        if (_postWord.Equals(""))
            return;
        
        Debug.Log("Sent message: " + _postWord);
        
        // 1. Strict prompt demanding 4 categories formatted line-by-line
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
        _response = _response.Replace("*", ""); // Remove markdown bolding if AI ignores instructions

        Debug.Log("Response from AI:\n" + _response);
        stackText = $"Gemini>\n{_response}\n{stackText}";

        // 2. Split the response by newlines to process category by category
        string[] lines = _response.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            // 3. Split each line by the colon (e.g., "Flavours" and " sweet, pandan, coconut")
            string[] categoryParts = line.Split(':');
            
            if (categoryParts.Length == 2)
            {
                string categoryName = categoryParts[0].Trim(); // e.g., "Flavours"
                string keywordString = categoryParts[1];       // e.g., " sweet, pandan, coconut"

                Debug.Log($"--- Parsed Category: {categoryName} ---");

                // 4. Split the keywords by commas
                string[] keywords = keywordString.Split(',');

                foreach (string keyword in keywords)
                {
                    string cleanKeyword = keyword.Trim();
                    Debug.Log($"Extracted Node Idea ({categoryName}): {cleanKeyword}");
                    
                    // Example implementation for your Mind Map Manager:
                    // FindObjectOfType<MindMapManager>().GenerateChildNode(selectedNode, cleanKeyword, categoryName);
                }
            }
        }
    }
}