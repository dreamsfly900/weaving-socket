﻿using cloud;
using MyInterface;
using P2P;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace 智信构建结构
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

         

        }

        private void Pserver_receiveeventbit(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
          
        }
        List<ServerPort> listsp = new List<ServerPort>();
        TCPcloud t = new TCPcloud();
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            if (txt_IP.Text!="" && txt_port.Text!="")
            {
                //验证IP地址和端口号的格式
                int portNum;
                bool isPort = Int32.TryParse(txt_port.Text, out portNum);
                if (Regex.IsMatch(txt_IP.Text.Trim(), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$") && isPort && portNum >= 0 && portNum <= 65535)
                {
                    String str = "|||"+txt_IP.Text+"|"+txt_port.Text;
                    if (radioButton2.Checked)
                    { str += "|web"; }
                    else
                    { str += "|"; }
                    //if (checkBox1.Checked)
                    //{ str += "|token"; }
                    //else
                    //{
                        str += "|";
                   // }
                    MyInterface.MyInterface mif = new MyInterface.MyInterface();
                    mif.Parameter = str.Split('|');
                   
                  
                    if (t.Run(mif))
                    {
                        t.AddProt(listsp);
                        lab_info.Text = "启动成功！";
                        timer1.Start();
                        //t.ReloadFlies();//重新加载插件
                    }
                }else
                {
                    lab_info.Text = "请检查IP地址或端口号的格式！";
                }

               
            }else
            {
                lab_info.Text = "IP地址和端口号不能为空！";
            }
       
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lab_info.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ServerPort sp = new ServerPort();
           
            if (checkBox1.Checked)
                sp.Istoken = true;
                sp.Port = Convert.ToInt32(txt_port.Text);
            if (radioButton2.Checked)
            {
                sp.PortType = portType.web;
                listBox1.Items.Add("WEB端口" + sp.Port);
            }
            else if (radioButton3.Checked)
            {
                sp.PortType = portType.http;
                listBox1.Items.Add("HTTP端口" + sp.Port);
            }
            else if (radioButton4.Checked)
            {
                sp.PortType = portType.bytes;

                Assembly [] abs = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly ab in abs)
                {
                    Type[] types = ab.GetExportedTypes();
                    foreach (Type t in types)
                    {
                        try
                        {
                            if (t.FullName== textBox1.Text)
                            {
                                sp.BytesDataparsing = ab.CreateInstance(textBox1.Text) as IDataparsing;
                            }
                        }
                        catch (Exception ex)
                        { throw ex; }
                    }
                }
            }
            else  
            {
                sp.PortType = portType.json;
                listBox1.Items.Add("json端口" + sp.Port);
            }
           

            listsp.Add(sp);
            panel2.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            foreach (var item in t.p2psevlist)
            {
                listBox2.Items.Add("端口："+ item.Port+"  连接人数："+ item.getNum());
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                panel2.Visible = true;
            }
            else
            {
                panel2.Visible = false;
            } 

        }
    }
}