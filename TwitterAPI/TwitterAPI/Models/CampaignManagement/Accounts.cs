using System;
using TwitterAPI.Services;

namespace TwitterAPI.Models.CampaignManagement
{
    public class Accounts
    {
        public static string GetAccount(string version)
        {
            return $"{version}/accounts/";
        }
        public string GetAccountId(string version, string account_id)
        {
            return $"{version}/accounts/{account_id}";
        }

        //public string PostAccount()
        //{
        //    return $"{version}/accounts/";
        //}
        //public string PutAccountId()
        //{
        //    return $"{version}/accounts/{account_id}";
        //}

        //public string DeleteAccountId()
        //{
        //    return $"{version}/accounts/{account_id}";
        //}
    }
}
