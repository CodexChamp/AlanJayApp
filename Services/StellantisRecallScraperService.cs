using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using HtmlAgilityPack;

namespace AlanJayApp.Services
{
    public class StellantisRecallScraperService
    {
        public async Task<string> CheckRecallRapidResponseAsync(string last8OfVin)
        {
            return await Task.Run(() =>
            {
                var options = new ChromeOptions();
                // options.AddArgument("--headless"); // If you want no browser UI
                options.AddArgument("--ignore-certificate-errors");

                using (var driver = new ChromeDriver(options))
                {
                    try
                    {
                        // 1. Construct the final URL with the VIN param
                        //    "USER" or other params might need real values or might be optional
                        string directUrl = $"https://wrecall.extra.chrysler.com/fleetreports/restricted/wrecall/RecallInq?requestorEmail1=&schedule2=on&vins={last8OfVin}&language=E&task=+&fan=99999&user=USER";

                        // 2. Navigate directly to that final URL
                        driver.Navigate().GoToUrl(directUrl);

                        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

                        // 3. Wait for the table to appear (assuming it's the same table)
                        wait.Until(ExpectedConditions.ElementIsVisible(
                            By.XPath("//table[@class='width100 cellspacing3 cellpadding1']")));

                        // 4. Parse the page source
                        string pageSource = driver.PageSource;
                        var doc = new HtmlDocument();
                        doc.LoadHtml(pageSource);

                        var tableNode = doc.DocumentNode.SelectSingleNode("//table[@class='width100 cellspacing3 cellpadding1']");
                        if (tableNode == null)
                        {
                            return "No recall table found in the HTML.";
                        }

                        var rows = tableNode.SelectNodes(".//tr");
                        if (rows == null || rows.Count == 0)
                        {
                            return "No rows found in the recall table.";
                        }

                        var recallResults = new List<RecallResult>();

                        foreach (var row in rows)
                        {
                            var cells = row.SelectNodes(".//td");
                            if (cells == null) continue;

                            // Check for "not found" row
                            if (cells.Count == 1 &&
                                cells[0].InnerText.Contains("Recall/Rapid Response not found", StringComparison.OrdinalIgnoreCase))
                            {
                                return $"No recall found for VIN (last 8): {last8OfVin}.";
                            }

                            // If there are 9 columns => recall data
                            if (cells.Count == 9)
                            {
                                recallResults.Add(new RecallResult
                                {
                                    VinLast8 = cells[0].InnerText.Trim(),
                                    VinFirst9 = cells[1].InnerText.Trim(),
                                    ItemCode = cells[2].InnerText.Trim(),
                                    Fan = cells[3].InnerText.Trim(),
                                    RecallNumber = cells[4].InnerText.Trim(),
                                    Description = cells[5].InnerText.Trim(),
                                    RecallDate = cells[6].InnerText.Trim(),
                                    DealerCode = cells[7].InnerText.Trim(),
                                    Name = cells[8].InnerText.Trim()
                                });
                            }
                        }

                        if (recallResults.Count == 0)
                        {
                            return "No recall data rows found (other than headers).";
                        }
                        else
                        {
                            return $"Found {recallResults.Count} recall row(s). First recall #: {recallResults[0].RecallNumber}";
                        }
                    }
                    catch (Exception ex)
                    {
                        return $"Error: {ex.Message}";
                    }
                }
            });
        }
    }

    public class RecallResult
    {
        public string? VinLast8 { get; set; }
        public string? VinFirst9 { get; set; }
        public string? ItemCode { get; set; }
        public string? Fan { get; set; }
        public string? RecallNumber { get; set; }
        public string? Description { get; set; }
        public string? RecallDate { get; set; }
        public string? DealerCode { get; set; }
        public string? Name { get; set; }
    }
}
