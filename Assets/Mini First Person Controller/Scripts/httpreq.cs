using UnityEngine;
using System.Net.Http;
using System.Text.RegularExpressions;
using TMPro;
using System.Threading.Tasks;
 
public class httpreq : MonoBehaviour
{
    private string[] lehrerURLs = new string[]
    {
        "https://www.htl-salzburg.ac.at/lehrerinnen-details/schweiberer-franz-prof-dipl-ing-c-205.html", 
        "https://www.htl-salzburg.ac.at/lehrerinnen-details/meerwald-stadler-susanne-prof-dipl-ing-g-009.html"
    };
 
    public TMP_Text lehrer1AusgabeFeld; 
    public TMP_Text lehrer2AusgabeFeld;
 
    private void Start()
    {
        LadeAlleLehrerDaten();
    }
 
    async void LadeAlleLehrerDaten()
    {
        int lehrerIndex = 0; 
        foreach (string url in lehrerURLs)
        {
            string daten = await HoleDatenVonURL(url);
 
            TMP_Text zielFeld = null;
            if (lehrerIndex == 0) zielFeld = lehrer1AusgabeFeld;
            if (lehrerIndex == 1) zielFeld = lehrer2AusgabeFeld;
            AktualisiereFeld(zielFeld, daten);
            lehrerIndex++; 
        }
    }
 
    async Task<string> HoleDatenVonURL(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode(); 
                return await response.Content.ReadAsStringAsync(); 
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Fehler beim Laden von {url}: {e.Message}");
                return "LADEFEHLER: Seite nicht gefunden."; 
            }
        }
    }
 
    void AktualisiereFeld(TMP_Text feld, string htmlContent)
    {
 
        string raumMuster = @"<div class=""field Raum"">.*?<span class=""text"">(.*?)<\/span>";
        string stundeMuster = @"<div class=""field SprStunde"">.*?<span class=""text"">(.*?)<\/span>";
 
        string raum = Regex.Match(htmlContent, raumMuster, RegexOptions.Singleline).Groups[1].Value.Trim();
        string stunde = Regex.Match(htmlContent, stundeMuster, RegexOptions.Singleline).Groups[1].Value.Trim();
 
        string nameMuster = @"<h1 class=""value"">\s*<span class=""text"">(.*?)<\/span>";
        string name = Regex.Match(htmlContent, nameMuster, RegexOptions.Singleline).Groups[1].Value.Trim();
 
        feld.text = $"{name}\n\n" +
                    $"Raum: {raum}\n" +
                    $"Sprechstunde: {stunde}";
    }
}