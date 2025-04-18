using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyThinkAds.ThirdParty.LitJson;

namespace AnyThinkAds.Api
{
    public class ATCallbackInfo
    {

        public readonly int network_firm_id;
        public readonly string adsource_id;
        public readonly int adsource_index;
        public readonly double adsource_price;
        public readonly int adsource_isheaderbidding;

        public readonly string id;
        public readonly double publisher_revenue;
        public readonly string currency;
        public readonly string country;
        public readonly string adunit_id;

        public readonly string adunit_format;
        public readonly string precision;
        public readonly string network_type;
        public readonly string network_placement_id;
        public readonly int ecpm_level;

        public readonly int segment_id;
        public readonly string scenario_id;
        public readonly string scenario_reward_name;
        public readonly int scenario_reward_number;

        public readonly string sub_channel;
        public readonly string channel;
        public readonly Dictionary<string, object> custom_rule;
        
        public readonly Dictionary<string, object> ext_info;
        public readonly Dictionary<string, object> user_load_extra_data;
        public readonly int abtest_id;

        public readonly string reward_custom_data;
        public readonly int placement_type;
        public readonly string shared_placement_id;
        public readonly string bid_floor;
        public readonly int dismiss_type;
        public readonly int ad_source_type;
        public readonly string ad_source_custom_ext;
        public readonly string network_name;
        public readonly string show_custom_ext;
        public readonly string e_c;
        public readonly int s_id;

        private string callbackJson;

        public ATCallbackInfo(string callbackJson)
        {
            try
            {
                this.callbackJson = callbackJson;


                JsonData jsonData = JsonMapper.ToObject(callbackJson);

                network_firm_id = int.Parse(jsonData.ContainsKey("network_firm_id") ? jsonData["network_firm_id"].ToString() : "0");
                adsource_id = jsonData.ContainsKey("adsource_id") ? (string)jsonData["adsource_id"] : "";
                adsource_index = int.Parse(jsonData.ContainsKey("adsource_index") ? jsonData["adsource_index"].ToString() : "-1");
                adsource_price = double.Parse(jsonData.ContainsKey("adsource_price") ? jsonData["adsource_price"].ToString() : "0");

                adsource_isheaderbidding = 0;
                if (jsonData.ContainsKey("adsource_isheaderbidding")) {
                    adsource_isheaderbidding = int.Parse(jsonData.ContainsKey("adsource_isheaderbidding") ? jsonData["adsource_isheaderbidding"].ToString() : "0");
                }

                id = jsonData.ContainsKey("id") ? (string)jsonData["id"] : "";
                publisher_revenue = double.Parse(jsonData.ContainsKey("publisher_revenue") ? jsonData["publisher_revenue"].ToString() : "0");
                currency = jsonData.ContainsKey("currency") ? (string)jsonData["currency"] : "";
                country = jsonData.ContainsKey("country") ? (string)jsonData["country"] : "";

                adunit_format = jsonData.ContainsKey("adunit_format") ? (string)jsonData["adunit_format"] : "";
                adunit_id = jsonData.ContainsKey("adunit_id") ? (string)jsonData["adunit_id"] : "";

                precision = jsonData.ContainsKey("precision") ? (string)jsonData["precision"] : "";

                network_type = jsonData.ContainsKey("network_type") ? (string)jsonData["network_type"] : "";

                network_placement_id = jsonData.ContainsKey("network_placement_id") ? (string)jsonData["network_placement_id"] : "";
                ecpm_level = int.Parse(jsonData.ContainsKey("ecpm_level") ? jsonData["ecpm_level"].ToString() : "0");
                abtest_id = int.Parse(jsonData.ContainsKey("abtest_id") ? jsonData["abtest_id"].ToString() : "0");
                segment_id = int.Parse(jsonData.ContainsKey("segment_id") ? jsonData["segment_id"].ToString() : "0");
                scenario_id = jsonData.ContainsKey("scenario_id") ? (string)jsonData["scenario_id"] : "";// RewardVideo & Interstitial

                if (jsonData.ContainsKey("user_load_extra_data")) {
                        user_load_extra_data = JsonMapper.ToObject<Dictionary<string, object>>(jsonData["user_load_extra_data"].ToJson());
                }

                scenario_reward_name = jsonData.ContainsKey("scenario_reward_name") ? (string)jsonData["scenario_reward_name"] : "";
                scenario_reward_number = int.Parse(jsonData.ContainsKey("scenario_reward_number") ? jsonData["scenario_reward_number"].ToString() : "0");

                channel = jsonData.ContainsKey("channel") ? (string)jsonData["channel"] : "";
                sub_channel = jsonData.ContainsKey("sub_channel") ? (string)jsonData["sub_channel"] : "";
                custom_rule = jsonData.ContainsKey("custom_rule") ? JsonMapper.ToObject<Dictionary<string, object>>(jsonData["custom_rule"].ToJson()) : null;
                ext_info = jsonData.ContainsKey("ext_info") ? JsonMapper.ToObject<Dictionary<string, object>>(jsonData["ext_info"].ToJson()) : null;

                reward_custom_data = jsonData.ContainsKey("reward_custom_data") ? (string)jsonData["reward_custom_data"] : "";

                placement_type = int.Parse(jsonData.ContainsKey("placement_type") ? jsonData["placement_type"].ToString() : "0");
                shared_placement_id = jsonData.ContainsKey("shared_placement_id") ? jsonData["shared_placement_id"].ToString() : "";
                bid_floor = jsonData.ContainsKey("bid_floor") ? jsonData["bid_floor"].ToString() : "";
                dismiss_type = int.Parse(jsonData.ContainsKey("dismiss_type") ? jsonData["dismiss_type"].ToString() : "0");
                ad_source_type = int.Parse(jsonData.ContainsKey("ad_source_type") ? jsonData["ad_source_type"].ToString() : "0");
                ad_source_custom_ext = jsonData.ContainsKey("ad_source_custom_ext") ? jsonData["ad_source_custom_ext"].ToString() : "";
                network_name = jsonData.ContainsKey("network_name") ? jsonData["network_name"].ToString() : "";
                show_custom_ext = jsonData.ContainsKey("show_custom_ext") ? jsonData["show_custom_ext"].ToString() : "";
                e_c = jsonData.ContainsKey("e_c") ? jsonData["e_c"].ToString() : "";
                s_id = int.Parse(jsonData.ContainsKey("s_id") ? jsonData["s_id"].ToString() : "0");
            }
            catch (System.Exception e) {
                System.Console.WriteLine("Exception caught: {0}", e);
            }
        }

        public string getOriginJSONString()
                {
            return callbackJson;
        }

        public Dictionary<string, object> toAdsourceDictionary()
        {
            Dictionary<string, object> dataDictionary = new Dictionary<string, object>();

            dataDictionary.Add("adsource_id", adsource_id);
            dataDictionary.Add("adsource_price", adsource_price);
            dataDictionary.Add("adunit_id", adunit_id);
            dataDictionary.Add("currency", currency);
            dataDictionary.Add("network_firm_id",network_firm_id);
            dataDictionary.Add("network_placement_id",network_placement_id);
            return dataDictionary;

        }

        public Dictionary<string, object> toDictionary()
        {
            Dictionary<string, object> dataDictionary = new Dictionary<string, object>();

            dataDictionary.Add("network_firm_id",network_firm_id);
            dataDictionary.Add("adsource_id", adsource_id);
            dataDictionary.Add("adsource_index", adsource_index);
            dataDictionary.Add("adsource_price", adsource_price);
            dataDictionary.Add("adsource_isheaderbidding", adsource_isheaderbidding);
            dataDictionary.Add("id", id);
            dataDictionary.Add("publisher_revenue", publisher_revenue);
            dataDictionary.Add("currency", currency);
            dataDictionary.Add("country", country);
            dataDictionary.Add("adunit_id", adunit_id);
            dataDictionary.Add("adunit_format", adunit_format);
            dataDictionary.Add("precision", precision);
            dataDictionary.Add("network_type", network_type);
            dataDictionary.Add("network_placement_id",network_placement_id);
            dataDictionary.Add("ecpm_level", ecpm_level);
            dataDictionary.Add("segment_id", segment_id);
            dataDictionary.Add("scenario_id", scenario_id);
            dataDictionary.Add("user_load_extra_data", user_load_extra_data);
            dataDictionary.Add("scenario_reward_name", scenario_reward_name);
            dataDictionary.Add("scenario_reward_number", scenario_reward_number);
            dataDictionary.Add("abtest_id", abtest_id);

            dataDictionary.Add("sub_channel", sub_channel);
            dataDictionary.Add("channel", channel);
            dataDictionary.Add("custom_rule", custom_rule);
            dataDictionary.Add("ext_info", ext_info);
            dataDictionary.Add("reward_custom_data", reward_custom_data);
            dataDictionary.Add("placement_type", placement_type);
            dataDictionary.Add("shared_placement_id", shared_placement_id);
            dataDictionary.Add("bid_floor", bid_floor);
            dataDictionary.Add("dismiss_type", dismiss_type);
            dataDictionary.Add("ad_source_type", ad_source_type);
            dataDictionary.Add("ad_source_custom_ext", ad_source_custom_ext);
            dataDictionary.Add("network_name", network_name);
            dataDictionary.Add("show_custom_ext", show_custom_ext);
            dataDictionary.Add("e_c", e_c);
            dataDictionary.Add("s_id", s_id);

            return dataDictionary;
        }


    }
}
