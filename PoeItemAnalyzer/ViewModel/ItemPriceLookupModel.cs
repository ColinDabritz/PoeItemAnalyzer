using ItemModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PoeItemAnalyzer.ViewModel
{
    class ItemPriceLookupModel
    {
        // for now, using same service call as poe_price_macro.ahk
        // from https://github.com/trackpete/exiletools-price-macro
        // URL scheme based on referenced code

        // https://github.com/trackpete/exiletools-price-macro/blob/master/poe_price_macro.ahk#L102
        public static readonly string ApiUrl = "http://api.exiletools.com/item-report-text";

        // https://github.com/trackpete/exiletools-price-macro/blob/master/poe_price_macro.ahk#L96
        public static readonly string RunVersion = "5.1";

        // TODO: figure out how to address user settings for leaguge, based on API results.
        // for now, hardcoded for testing.
        // https://github.com/trackpete/exiletools-price-macro/blob/master/poe_price_macro.ahk#L83
        //public static readonly string LeagueName = "standard";
        //public static readonly string LeagueName = "hardcore";
        public static readonly string LeagueName = "prophecy";

        // https://github.com/trackpete/exiletools-price-macro/blob/master/poe_price_macro.ahk#L92
        public static readonly string ShowDays = "7";
        
        public static async Task<string> GetItemPriceFromWeb(LootItem item)
        {
            var client = new HttpClient();

             var itemData = Uri.EscapeDataString(item.RawItemText);
            //var itemData = AhkScriptUrlEscaping(item.RawItemText.Trim());

            // https://github.com/trackpete/exiletools-price-macro/blob/master/poe_price_macro.ahk#L168
            var ApiPostData = $"v={RunVersion}&itemData={itemData}&league={LeagueName}&showDays={ShowDays}";
            
            var content = new StringContent(ApiPostData);

            // the current API chokes if the Content-Type header is included.
            content.Headers.Remove("Content-Type"); 

            HttpResponseMessage response = await client.PostAsync(ApiUrl, content);

            if(response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Error '{response.StatusCode}' retrieving price information. Error message: {await response.Content.ReadAsStringAsync()}";
            }
        }

        public static string AhkScriptUrlEscaping(string input)
        {
            // mimicing https://github.com/trackpete/exiletools-price-macro/blob/master/poe_price_macro.ahk#L252
            var unixOnlyEndings = input.Replace("\r","");
            var initialUri = Uri.EscapeDataString(unixOnlyEndings);
            var useRealParensForSomeReason = initialUri.Replace("%28", "(").Replace("%29", ")");

            return useRealParensForSomeReason;
        }
    }
}
