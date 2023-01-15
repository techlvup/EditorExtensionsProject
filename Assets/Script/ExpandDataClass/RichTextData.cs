using UnityEngine;

[System.Serializable]
public class RichTextData
{
    public string text = "";
    public int fontSize = 20;
    public Color color = Color.white;
    public TextAnchor align = TextAnchor.MiddleCenter;
    public FontStyle fontStyle = FontStyle.Normal;

    public bool isSetColor = false;
    public bool isSetFontStyle = false;
    public bool isSetFontSize = false;

    public GUIStyle gUIStyle = new GUIStyle();

    public RichTextData()
    {
        gUIStyle.richText = true;
    }
}