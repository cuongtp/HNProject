using HNProject.Controllers;
using HNProject.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DemoSignalR.Hubs
{
    
    public class NotifyHub : Hub
    {
        private GoStoreDbContext _context;
        //goi 1 thang   
        public void NotifyShipper(string connectionIdCuaThangClientDo, string id_order, int count, string msg, string id_shipper, double lat, double @long)
        {
            string thamSoGiDoCuaHamGiDoDuoiClient2 = msg;

            ////goi tat ca
            //Clients.All.tenHamNaoDoDuoiClient2("goi tat ca o day: " + 
            //    thamSoGiDoCuaHamGiDoDuoiClient2);
            //goi chi dinh t` nao day
            
            Clients.Client(connectionIdCuaThangClientDo).NotifyShipper(id_order,count,msg,id_shipper,lat,@long);
        }

        public void NotifyCustomer(string connectionIdCuaThangClientDo,string msg)
        {
            Clients.Client(connectionIdCuaThangClientDo).NotifyCustomer(msg);
        }

        
        //public override Task OnConnected()
        //{
        //    return base.OnConnected();
        //}

        //public override Task OnDisconnected(bool stopCalled)
        //{
        //    return base.OnDisconnected(stopCalled);
        //}

        public void InRaGiDo(string gido, string connectionID)
        {
            gido = "Tim thay mot shipper!";
            string thamSoGiDoCuaHamGiDoDuoiClient2 = gido;


            string connectionID_shipper = connectionID;
            ////goi tat ca
            //Clients.All.tenHamNaoDoDuoiClient2("goi tat ca o day: " + 
            //    thamSoGiDoCuaHamGiDoDuoiClient2);
            //goi chi dinh t` nao day
            Clients.Client(connectionID_shipper).tenHamNaoDoDuoiClient2(new Random().Next() + ":" + thamSoGiDoCuaHamGiDoDuoiClient2);
        }


        public void ShipperNotFound(string connectionIdCuaThangClientDo, string id_order, int count, string msg, string id_shipper, double lat, double @long)
        {
            string thamSoGiDoCuaHamGiDoDuoiClient2 = msg;

            ////goi tat ca
            //Clients.All.tenHamNaoDoDuoiClient2("goi tat ca o day: " + 
            //    thamSoGiDoCuaHamGiDoDuoiClient2);
            //goi chi dinh t` nao day

            Clients.Client(connectionIdCuaThangClientDo).ShipperNotFound(id_order, count, msg, id_shipper, lat, @long);
        }

    }
}