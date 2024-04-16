using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCollection : MonoBehaviour{
    private string fileName = @"/home/brevin/Documents/seniorProject/data.txt";
    List<string> data = new List<string>();
    float time = 0;

    void Start(){
        CreateFile();
    }

    void Update(){
        time += Time.deltaTime;
    }

    void CreateFile(){
        if (!(File.Exists(fileName))){
            FileStream fs = File.Create(fileName);
        }
        else{
            File.Delete(fileName);
            FileStream fs = File.Create(fileName);
        }
    
    }   
    
    public void CollectData(string creature, string causeOfDeath){
        string timeString = string.Format("{0:N3}", time);
        string dataEntry = creature + "," + causeOfDeath + "," + timeString;
        data.Add(dataEntry);
    }


    public void WriteFile(){
        if (File.Exists(fileName)){
            using (StreamWriter sw = new StreamWriter(fileName)){
                for(int i = 0; i < data.Count; i++){
                    sw.WriteLine(data[i]);
                    sw.WriteLine('\n');
                }
                sw.Close();
            }
        }
    }
}
