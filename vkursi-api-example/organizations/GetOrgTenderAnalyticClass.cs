﻿using System;
using RestSharp;
using Newtonsoft.Json;
using vkursi_api_example.token;
using System.Collections.Generic;

namespace vkursi_api_example.organizations
{
    public class GetOrgTenderAnalyticClass
    {
        /*
        
        43. Загальна характеристика по тендерам
        [POST] /api/1.0/organizations/getorgtenderanalytic    
        
        curl --location --request POST 'https://vkursi-api.azurewebsites.net/api/1.0/organizations/getorgtenderanalytic' \
        --header 'ContentType: application/json' \
        --header 'Authorization: Bearer eyJhbGciOiJIUzI1Ni...' \
        --header 'Content-Type: application/json' \
        --data-raw '{"Edrpou":["00131305"]}'
         
        */

        public static GetOrgTenderAnalyticResponseModel GetOrgTenderAnalytic(string token, string edrpou)
        {
            if (String.IsNullOrEmpty(token))
                token = AuthorizeClass.Authorize();

            string responseString = string.Empty;

            while (string.IsNullOrEmpty(responseString))
            {
                GetOrgTenderAnalyticRequestBodyModel GOTARequestBody = new GetOrgTenderAnalyticRequestBodyModel
                {
                    Edrpou = new List<string>
                    {
                        edrpou
                    }
                };

                string body = JsonConvert.SerializeObject(GOTARequestBody);

                // Example Body: {"Edrpou":["00131305"]}

                RestClient client = new RestClient("https://vkursi-api.azurewebsites.net/api/1.0/organizations/getorgtenderanalytic");
                RestRequest request = new RestRequest(Method.POST);

                request.AddHeader("ContentType", "application/json");
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                responseString = response.Content;

                if ((int)response.StatusCode == 401)
                {
                    Console.WriteLine("Не авторизований користувач або закінчився термін дії токену. Отримайте новый token на api/1.0/token/authorize");
                    token = AuthorizeClass.Authorize();
                }

                else if ((int)response.StatusCode != 200 || response.ErrorMessage == "The operation has timed out.")
                {
                    Console.WriteLine("Запит не успішний");
                    return null;
                }
            }

            GetOrgTenderAnalyticResponseModel GOTAResponseRow = new GetOrgTenderAnalyticResponseModel();

            GOTAResponseRow = JsonConvert.DeserializeObject<GetOrgTenderAnalyticResponseModel>(responseString);

            return GOTAResponseRow;
        }
    }


    /*
     
        // Python - http.client example:

        import http.client
        import mimetypes
        conn = http.client.HTTPSConnection("vkursi-api.azurewebsites.net")
        payload = "{\"Edrpou\":[\"00131305\"]}"
        headers = {
          'ContentType': 'application/json',
          'Authorization': 'Bearer eyJhbGciOiJIUzI1Ni...',
          'Content-Type': 'application/json',
        }
        conn.request("POST", "/api/1.0/organizations/getorgtenderanalytic", payload, headers)
        res = conn.getresponse()
        data = res.read()
        print(data.decode("utf-8"))


        // Java - OkHttp example:
        
        OkHttpClient client = new OkHttpClient().newBuilder()
          .build();
        MediaType mediaType = MediaType.parse("application/json");
        RequestBody body = RequestBody.create(mediaType, "{\"Edrpou\":[\"00131305\"]}");
        Request request = new Request.Builder()
          .url("https://vkursi-api.azurewebsites.net/api/1.0/organizations/getorgtenderanalytic")
          .method("POST", body)
          .addHeader("ContentType", "application/json")
          .addHeader("Authorization", "Bearer eyJhbGciOiJIUzI1Ni...")
          .addHeader("Content-Type", "application/json")
          .build();
        Response response = client.newCall(request).execute();

    */



    public class GetOrgTenderAnalyticRequestBodyModel                                   // Модель запиту 
    {
        public List<string> Edrpou { get; set; }                                        // Перелік кодів ЄДРПОУ (обмеження 1)
    }

    public class GetOrgTenderAnalyticResponseModel                                      // Модель на відповідь
    {
        public bool IsSucces { get; set; }                                              // Статус відповіді по API
        public string Succes { get; set; }                                              // Чи успішний запит (maxLength:128)
        public List<OrgTenderAnalyticApiAnswerModelData> Data { get; set; }             // Перелік даних
    }

    public class OrgTenderAnalyticApiAnswerModelData                                    // Перелік даних
    {
        public string Edrpou { get; set; }                                              // Код ЄДРПОУ (maxLength:12)
        public int Proposals { get; set; }                                              // Подано заявок
        public int Customers { get; set; }                                              // Замовники
        public int Wins { get; set; }                                                   // Перемоги
        public decimal Sum { get; set; }                                                // Сума
        public decimal Competitions { get; set; }                                       // Конкуренція
        public int Disqualification { get; set; }                                       // Дискваліфікації
    }
}
