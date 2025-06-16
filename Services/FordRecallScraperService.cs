using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using HtmlAgilityPack;

namespace AlanJayApp.Services
{
    public class FordRecallScraperService
    {
        // Example method: logs in, navigates to recall page, enters VIN, and returns recall info
        public async Task<string> LoginAndScrapeRecallsAsync(string vin)
        {
            // Hard-coded credentials for demonstration ONLY. 
            // In production, use a secure storage solution (e.g., Azure Key Vault, user secrets, environment variables).
            string username = "s-wil384";
            string password = "Fordsucks05!";

            // The initial login URL you mentioned
            string loginUrl = "https://www.faust.idp.ford.com/Federation/SAMLSSO?issuer=urn:wslxidp:cookie:to:saml";

            return await Task.Run(() =>
            {
                // 1. Configure ChromeDriver
                var options = new ChromeOptions();
                // options.AddArgument("--headless"); // Uncomment if running on a headless server
                options.AddArgument("--ignore-certificate-errors");

                using (var driver = new ChromeDriver(options))
                {
                    try
                    {
                        // 2. Go to the Ford SSO login page
                        driver.Navigate().GoToUrl(loginUrl);

                        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

                        // 3. Enter username
                        var userField = wait.Until(ExpectedConditions
                            .ElementIsVisible(By.Id("userName")));  // Adjust if it's different
                        userField.Clear();
                        userField.SendKeys(username);

                        // 4. Enter password
                        var passField = wait.Until(ExpectedConditions
                            .ElementIsVisible(By.Id("password"))); // Adjust if it's different
                        passField.Clear();
                        passField.SendKeys(password);

                        // 5. Click "Login"
                        var loginButton = wait.Until(ExpectedConditions
                            .ElementToBeClickable(By.Id("btn-sign-in"))); // Adjust if it's different
                        loginButton.Click();

                        // 6. Now you see the screen: "You are signed in. Sign in to one of the following sites."
                        //    Wait for that dropdown to appear, then select "@Ford Online (PROD-Alias)"
                        //    The HTML might have a <select> with a name or id, or it could be something else.

                        // Wait for the dropdown
                        // Example: By.Id("siteDropdown") — replace with the real ID or name
                        var siteDropdown = wait.Until(ExpectedConditions
                            .ElementIsVisible(By.Name("siteList")));
                        // Use Selenium's SelectElemen// The visible text in the dropdown

                        // 7. Click the second "Sign In" button
                        // Example: an <input type="submit" value="Sign in" ...> or a <button> 
                        var secondSignInButton = wait.Until(ExpectedConditions
                            .ElementToBeClickable(By.XPath("//input[@value='Sign in']")));
                        secondSignInButton.Click();

                        // 8. Now you're in the Ford Online portal. 
                        //    Look for the link or button to get to the recall lookup page.
                        //    The following is an EXAMPLE. You must adjust to the actual link text or element:

                        var recallLink = wait.Until(ExpectedConditions
                            .ElementToBeClickable(By.LinkText("Recalls")));
                        recallLink.Click();

                        // 9. Wait for the recall page to load
                        //    If the URL changes, you can wait for part of the new URL:
                        wait.Until(ExpectedConditions.UrlContains("recallPage"));

                        // 10. Enter the VIN in a text box
                        var vinField = wait.Until(ExpectedConditions
                            .ElementIsVisible(By.Id("vinLookup"))); // Adjust ID as needed
                        vinField.Clear();
                        vinField.SendKeys(vin);

                        // 11. Click a "Search" or "Lookup" button
                        var searchButton = wait.Until(ExpectedConditions
                            .ElementToBeClickable(By.Id("searchButton"))); // Adjust ID as needed
                        searchButton.Click();

                        // 12. Wait for results to appear
                        wait.Until(ExpectedConditions
                            .ElementIsVisible(By.Id("resultsTable"))); // or whatever indicates the results are ready

                        // 13. Parse the page to extract recall info
                        string pageSource = driver.PageSource;
                        var doc = new HtmlDocument();
                        doc.LoadHtml(pageSource);

                        // Example: find a recall info node
                        // Adjust XPath or ID to match the real results
                        var recallNode = doc.DocumentNode.SelectSingleNode("//div[@id='recall-info']");
                        if (recallNode != null)
                        {
                            return "Recall Info: " + recallNode.InnerText.Trim();
                        }
                        else
                        {
                            return "No recall info found for VIN: " + vin;
                        }
                    }
                    catch (Exception ex)
                    {
                        return $"Error during login/recall scrape: {ex.Message}";
                    }
                }
            });
        }

        // Example leftover from your original code (scraping a window sticker).
        // You could remove this if you don't need window sticker details anymore.
        public async Task<string> GetWindowStickerDetailsAsync(string vin)
        {
            // Wrap Selenium calls in Task.Run if you need to avoid blocking in Blazor
            return await Task.Run(() => ScrapeWindowSticker(vin));
        }

        // Private method for window sticker scraping (as an example placeholder).
        private string ScrapeWindowSticker(string vin)
        {
            // If you have a separate site for window sticker details, put that logic here.
            // For demonstration only:
            return $"Window sticker details for VIN: {vin} (not yet implemented).";
        }
    }
}
