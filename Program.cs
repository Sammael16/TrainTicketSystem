using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace MyApi
{
    class Program
    {
        static void Main(string[] args)
        {   
            /*URL üzerinden bir JSON post verisini okuma.(UYARLANABİLİR)
            var client = new HttpClient();  
            var endpoint = new Uri("https://jsonplaceholder.typicode.com/posts");
            var result = client.GetAsync(endpoint).Result;
            var StrJson = result.Content.ReadAsStringAsync().Result;
            Console.Write(StrJson);*/
            
              
              //Deneme input verisi.
              var data = @"{
    ""Tren"":
    {
        ""Ad"":""Başkent Ekspres"",
        ""Vagonlar"":
        [
            {""Ad"":""Vagon 1"", ""Kapasite"":100, ""DoluKoltukAdet"":65},
            {""Ad"":""Vagon 2"", ""Kapasite"":90, ""DoluKoltukAdet"":80},
            {""Ad"":""Vagon 3"", ""Kapasite"":80, ""DoluKoltukAdet"":60}
        ]
    },
    ""RezervasyonYapilacakKisiSayisi"":10,
    ""KisilerFarkliVagonlaraYerlestirilebilir"":false
}";

        var inputData = JsonConvert.DeserializeObject<GotJson>(data);//Input JSON verisi okuma.

        ReturnJson Answerdata = new ReturnJson();//Output JSON verisi objesi

        //Kişiler farklı vagonlara yerleştirilemez ise aşağıdaki algoritma çalışacak.
        if(!inputData.KisilerFarkliVagonlaraYerlestirilebilir){
            Answerdata.RezervasyonYapilabilir = false;
        for (int i=0;i<inputData.Tren.Vagonlar.Count;i++){//Her bir vagon incelenip bütün herkesin yerleştirilebileceği vagon arama.
            if((inputData.Tren.Vagonlar[i].DoluKoltukAdet+inputData.RezervasyonYapilacakKisiSayisi)>=(int)(inputData.Tren.Vagonlar[i].Kapasite*0.7)){//%70 kuralı
                continue;
            }
            else{//Yerleştirilecek vagon bulunduğunda yapılan işlem.
                List<Yerlesim> yerlesims = new List<Yerlesim>();
                var yerlesim=new Yerlesim();
                yerlesim.VagonAdi = inputData.Tren.Vagonlar[i].Ad;
                yerlesim.KisiSayisi = inputData.RezervasyonYapilacakKisiSayisi;
                yerlesims.Add(yerlesim);
                Answerdata.RezervasyonYapilabilir = true;
                Answerdata.YerlesimAyrinti=yerlesims;
                break;
            }}}


        //Kişiler farklı vagonlara yerleştirilebilir ise aşağıdaki algoritma çalışacak.
        else if(inputData.KisilerFarkliVagonlaraYerlestirilebilir){
            Answerdata.RezervasyonYapilabilir = false;
            List<Yerlesim> yerlesims = new List<Yerlesim>();
            
            for (int i=0;i<inputData.Tren.Vagonlar.Count;i++){//Her bir vagon itere edilmekte.
                if((inputData.Tren.Vagonlar[i].DoluKoltukAdet)>=(inputData.Tren.Vagonlar[i].Kapasite*0.7)){
                continue;
            }
                else{//Boş vagon bulunduğunda o vagona kişileri yerleştirip kalan kişiler için diğer vagonlar inceleniyor.
                    var yerlesim=new Yerlesim();
                    int tempkisi=inputData.RezervasyonYapilacakKisiSayisi;
                    int ParcaRezervasyon = (int)(Math.Round((inputData.Tren.Vagonlar[i].Kapasite*0.7))-(inputData.Tren.Vagonlar[i].DoluKoltukAdet));
                    inputData.RezervasyonYapilacakKisiSayisi -= ParcaRezervasyon;
            
                    if(inputData.RezervasyonYapilacakKisiSayisi>0){
                        yerlesim.VagonAdi=inputData.Tren.Vagonlar[i].Ad;
                        yerlesim.KisiSayisi=ParcaRezervasyon;
                        yerlesims.Add(yerlesim);
                    }
                    else{
                        yerlesim.VagonAdi=inputData.Tren.Vagonlar[i].Ad;
                        yerlesim.KisiSayisi=tempkisi;
                        yerlesims.Add(yerlesim);
                    }
                   
                    if(inputData.RezervasyonYapilacakKisiSayisi <= 0){
                        Answerdata.RezervasyonYapilabilir = true;
                        Answerdata.YerlesimAyrinti = yerlesims;
                        break;}
                }
                }}

                /*TEST OUTPUTS.
                Console.WriteLine(Answerdata.RezervasyonYapilabilir);
                for(int i=0;i<Answerdata.YerlesimAyrinti.Count;i++){
                Console.WriteLine(Answerdata.YerlesimAyrinti[i].KisiSayisi);
                Console.WriteLine(Answerdata.YerlesimAyrinti[i].VagonAdi);}*/

                /*JSON formatındaki output verisini URL ye gönderme(UYARLANABİLİR).
                string output = JsonConvert.SerializeObject(Answerdata);//Serializing to JSON.

                var content = new StringContent(output, System.Text.Encoding.UTF8, "application/json");
                client.PostAsync(endpoint, content);*/
    
    }}

    //GİRDİ JSON VERİLERİ//
    public class Vagon{
        public string Ad {get;set;}
        public int Kapasite {get;set;}
        public int DoluKoltukAdet {get;set;}
    }
    public class TrainJson{
        public string Ad{get;set;}
        public List<Vagon> Vagonlar{get;set;}
    }
    public class GotJson{
        public TrainJson Tren{get;set;}
        public int RezervasyonYapilacakKisiSayisi{get;set;}
        public bool KisilerFarkliVagonlaraYerlestirilebilir{get;set;}

    }
    //ÇIKTI JSON VERİLERİ//
    public class ReturnJson{
        public bool RezervasyonYapilabilir {get;set;}
        public List<Yerlesim> YerlesimAyrinti {get;set;}
    }
    public class Yerlesim{
        public string VagonAdi {get;set;}
        public int KisiSayisi {get;set;}
    }
}
