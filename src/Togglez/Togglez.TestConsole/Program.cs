﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Org.Apache.Zookeeper.Data;
using ZooKeeperNet;

namespace Togglez.TestConsole
{
    class Program 
    {
        //need to create this setting in zk first. It should have a json payload with {foo:someInteger}.
        private const string _path = "/testharness";

        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            var zk = ZkRunner.New()
                .Path(() => _path)
                .ConnectionString(() => "192.168.60.2:2181")
                .SessionTimeout(() => TimeSpan.FromSeconds(3))
                .Build();

            var togglez = zk.Start();


            //waiting synchronously is optional. however, Get<> calls will return nulls / defaults until settings are ready.
            //using subscriptions where possible. subscriptions run on first connect, and everytime the path's value changes.
            //you only need to subcribe to a single setting for any particular path. In the handler, you can use togglez.Get<>(..) to
            //get the other values for the same path. 
            //Get<> doesn't make a call to zk...it simply uses state that's already been fetched. As such, you either need to use
            //a subscription, or wait for settings for one time init.
            Console.WriteLine("Waiting for settings...");
            togglez.WaitForFirstSettings(TimeSpan.FromSeconds(10));
            Console.WriteLine("Got settings...");

            Console.WriteLine(togglez.Get<int>("foo"));
            togglez.SubscribeOn<int>("foo", Console.WriteLine);
            Console.WriteLine(togglez.Get<int>("foo"));


            

            Console.ReadLine();
            zk.Dispose();
        }

    }
}
