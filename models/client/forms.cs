using System.Net;

namespace Forms;

public class HeaderModel{
    public HeaderModel(string Authorization){
        this.Authorization = Authorization;
    }
    string Authorization {get;}
}