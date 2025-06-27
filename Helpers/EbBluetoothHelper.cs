using System;
using System.Linq;
using System.Text;
using Android.Bluetooth;
using Java.Util;
using System.IO;
using System.Threading.Tasks;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Droid.Helpers;
using Xamarin.Forms;
using ExpressBase.Mobile.Models;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Drawing;
using Android.OS;
using Android;
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Android.Content.PM;
using Android.Content;
using ExpressBase.Mobile.Constants;
using ExpressBase.Mobile.Data;
using Newtonsoft.Json;
using ExpressBase.Mobile.Extensions;

[assembly: Dependency(typeof(EbBluetoothHelper))]

namespace ExpressBase.Mobile.Droid.Helpers
{
    public class EbBluetoothHelper : IEbBluetoothHelper
    {
        private BluetoothSocket _socket;
        private Stream _outputStream;

        public bool RequestBluetoothPermissions()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                string[] permissions = new string[]
                {
                    Manifest.Permission.BluetoothScan,
                    Manifest.Permission.BluetoothConnect
                };

                foreach (var permission in permissions)
                {
                    if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, permission) != Permission.Granted)
                    {
                        ActivityCompat.RequestPermissions(MainActivity.Instance, permissions, 1001);
                        return false;
                    }
                }
            }
            return true;
        }

        public bool EnableAndCheckBluetoothAdapter()
        {
            try
            {
                BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
                if (adapter == null || !adapter.IsEnabled)
                {
                    Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                    MainActivity.Instance.StartActivityForResult(enableBtIntent, 1);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ConnectToPrinter(string printerName)
        {
            try
            {
                BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
                if (adapter == null || !adapter.IsEnabled)
                {
                    throw new Exception("Bluetooth is not available or not enabled.");
                }

                BluetoothDevice device = (from bd in adapter.BondedDevices
                                          where bd.Name == printerName
                                          select bd).FirstOrDefault();

                if (device == null)
                {
                    throw new Exception("Printer not found.");
                }

                UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"); // Standard SPP UUID
                _socket = device.CreateRfcommSocketToServiceRecord(uuid);
                await _socket.ConnectAsync();

                _outputStream = _socket.OutputStream;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection Error: " + ex.Message);
                return false;
            }
        }

        public async Task<List<EbBTDevice>> GetBluetoothDeviceList()
        {
            List<EbBTDevice> BtList = new List<EbBTDevice>();
            try
            {
                BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
                foreach (BluetoothDevice bd in adapter.BondedDevices)
                {
                    BtList.Add(new EbBTDevice() { DeviceName = bd.Name, Address = bd.Address });
                }
            }
            catch (Exception e)
            {
                Utils.Toast("Failed to fetch BT devices: " + e.Message);
            }
            return BtList;
        }

        public async Task PrintText(string text)
        {
            if (_outputStream == null)
            {
                Utils.Toast("Printer not connected.");
            }

            //byte[] buffer = Encoding.UTF8.GetBytes(text + "\n");
            //await _outputStream.WriteAsync(buffer, 0, buffer.Length);
            //await _outputStream.FlushAsync();

            await Task.Delay(1000);
            byte[] initCommand = new byte[] { 0x1B, 0x40 }; // ESC @ (Initialize Printer)
            byte[] textBytes = Encoding.UTF8.GetBytes(text + "\r\n");
            byte[] printCommand = new byte[] { 0x0A }; // Line Feed

            await _outputStream.WriteAsync(initCommand, 0, initCommand.Length);
            await _outputStream.WriteAsync(textBytes, 0, textBytes.Length);
            await _outputStream.WriteAsync(printCommand, 0, printCommand.Length);
        }

        public void Disconnect()
        {
            _outputStream?.Close();
            _socket?.Close();
        }

        //===========================================================

        public async Task<bool> PrintInvoice(string refid, string param)
        {
            bool connected = false;
            try
            {
                EbPrintLayout layout = (EbPrintLayout)EbPageHelper.GetWebObjects(refid);
                if (layout == null)
                {
                    Utils.Toast("Print layout configuration not found");
                    return false;
                }
                App.Settings.SelectedBtDevice = Store.GetJSON<EbBTDevice>(AppConst.CURRENT_BT_PRINTER);

                if (App.Settings.SelectedBtDevice?.DeviceName == null)
                {
                    Utils.Toast("Please select a printer");
                    return false;
                }

                string query = layout.OfflineQuery.GetCode();
                List<Param> _p = JsonConvert.DeserializeObject<List<Param>>(param);
                EbDataSet ds = App.DataDB.DoQueries(query, _p.ToDbParams().ToArray());

                if (!(ds.Tables.Count == 2 && ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0) && layout.ThermalPrintTemplate == ThermalPrintTemplates.SalesInvoiceV1)
                {
                    Utils.Toast("Failed to fetch data for printing(1)");
                    return false;
                }
                else if (!(ds.Tables.Count > 2 && ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0) && layout.ThermalPrintTemplate == ThermalPrintTemplates.SalesInvoiceV2)
                {
                    Utils.Toast("Failed to fetch data for printing(2)");
                    return false;
                }

                EbDataRow dr = ds.Tables[0].Rows[0];
                InvoiceData _data = null;
                PrintData _data2 = null;

                if (layout.ThermalPrintTemplate == ThermalPrintTemplates.SalesInvoiceV1)
                {
                    _data = new InvoiceData()
                    {
                        store_name = (string)getCellValue(dr, "store_name", typeof(string)),
                        store_address = (string)getCellValue(dr, "store_address", typeof(string)),
                        store_vattrn = (string)getCellValue(dr, "store_vattrn", typeof(string)),
                        store_whatsapp = (string)getCellValue(dr, "store_whatsapp", typeof(string)),
                        customer_details = (string)getCellValue(dr, "customer_details", typeof(string)),
                        invoice_details = (string)getCellValue(dr, "invoice_details", typeof(string)),
                        amt_received = (double)getCellValue(dr, "amt_received", typeof(double)),
                        closing_balance = (double)getCellValue(dr, "closing_balance", typeof(double))
                    };

                    foreach (EbDataRow row in ds.Tables[1].Rows)
                    {
                        _data.sal_items.Add(new InvoiceDataLines()
                        {
                            item_name = (string)getCellValue(row, "item_name", typeof(string)),
                            rate = (double)getCellValue(row, "rate", typeof(double)),
                            vat_per = (double)getCellValue(row, "vat_per", typeof(double)),
                            qty = (int)getCellValue(row, "qty", typeof(int)),
                            rtn = (string)getCellValue(row, "rtn", typeof(string)),
                            net_amt = (double)getCellValue(row, "net_amt", typeof(double)),
                            vat_amt = (double)getCellValue(row, "vat_amt", typeof(double)),
                            amt = (double)getCellValue(row, "amt", typeof(double))
                        });
                    }
                }
                else if (layout.ThermalPrintTemplate == ThermalPrintTemplates.SalesInvoiceV2)
                {
                    _data2 = GetPrintData(ds);
                }

                connected = await this.ConnectToPrinter(App.Settings.SelectedBtDevice.DeviceName);

                if (connected)
                {
                    if (layout.ThermalPrintTemplate == ThermalPrintTemplates.SalesInvoiceV1)
                        await this.PrintPageSV_V1(_data);
                    else if (layout.ThermalPrintTemplate == ThermalPrintTemplates.SalesInvoiceV2)
                        await this.PrintPageSV_V2(_data2);

                    this.Disconnect();
                    return true;
                }
                else
                {
                    Utils.Toast("Failed to connect to printer.");
                }
            }
            catch (Exception ex)
            {
                Utils.Toast("Error: " + ex.Message);
                EbLog.Error("Error in [EbBluetoothHelper.PrintInvoice] " + ex.Message + " " + ex.StackTrace);
                if (connected)
                    this.Disconnect();
            }
            return false;
        }

        private object getCellValue(EbDataRow row, string colName, Type type)
        {
            if (type == typeof(int))
                return row[colName] == null ? 0 : Convert.ToInt32(row[colName]);

            if (type == typeof(double))
                return row[colName] == null ? 0.0 : Convert.ToDouble(row[colName]);

            return row[colName] == null ? string.Empty : row[colName];
        }

        private class InvoiceData
        {
            public string store_name { get; set; }
            public string store_address { get; set; }
            public string store_vattrn { get; set; }
            public string store_whatsapp { get; set; }
            public string customer_details { get; set; }
            public string invoice_details { get; set; }
            public double amt_received { get; set; }
            public double closing_balance { get; set; }
            public List<InvoiceDataLines> sal_items { get; set; }

            public InvoiceData()
            {
                sal_items = new List<InvoiceDataLines>();
            }
        }

        private class InvoiceDataLines
        {
            public string item_name { get; set; }
            public double rate { get; set; }
            public double vat_per { get; set; }
            public int qty { get; set; }
            public string rtn { get; set; }
            public double net_amt { get; set; }
            public double vat_amt { get; set; }
            public double amt { get; set; }
        }

        private class PrintData
        {
            public int page_width { get; set; }//in characters
            public string divider_char_header { get; set; }
            public string divider_char_table { get; set; }
            public string rhead_main1 { get; set; }
            public string rhead_sub1 { get; set; }
            public string rhead_sub2 { get; set; }
            public string rhead_sub3 { get; set; }

            public string rhead_inner1 { get; set; }
            public int rhead_left_width { get; set; }
            public string rhead_text_left { get; set; }
            public string rhead_text_right { get; set; }
            public string rhead_text { get; set; }

            public PrintDataHeader table1_meta { get; set; }
            public PrintDataHeader table2_meta { get; set; }
            public PrintDataHeader table3_meta { get; set; }

            public List<PrintDataLines> table1 { get; set; }
            public List<PrintDataLines> table2 { get; set; }
            public List<PrintDataLines> table3 { get; set; }

            public int rfoot_left_width { get; set; }
            public string rfoot_text_left { get; set; }
            public string rfoot_text_right { get; set; }
            public string rfoot_text { get; set; }

            public PrintData()
            {
                table1_meta = new PrintDataHeader();
                table2_meta = new PrintDataHeader();
                table3_meta = new PrintDataHeader();
                table1 = new List<PrintDataLines>();
                table2 = new List<PrintDataLines>();
                table3 = new List<PrintDataLines>();
            }
        }

        private class PrintDataHeader
        {
            public string dhead_text { get; set; }
            public string dfoot_text { get; set; }

            public string th_left1 { get; set; }
            public string th_left2 { get; set; }
            public string th_left3 { get; set; }
            public string th_right1 { get; set; }
            public string th_right2 { get; set; }
            public string th_right3 { get; set; }
            public string th_right4 { get; set; }
            public string th_right5 { get; set; }
            public string th_right6 { get; set; }
            public string th_right7 { get; set; }
            public string th_right8 { get; set; }
        }

        private class PrintDataLines
        {
            public string td_left1 { get; set; }
            public string td_left2 { get; set; }
            public string td_left3 { get; set; }
            public string td_right1 { get; set; }
            public string td_right2 { get; set; }
            public string td_right3 { get; set; }
            public string td_right4 { get; set; }
            public string td_right5 { get; set; }
            public string td_right6 { get; set; }
            public string td_right7 { get; set; }
            public string td_right8 { get; set; }
        }

        private PrintData GetPrintData(EbDataSet ds)
        {
            EbDataRow dr = ds.Tables[0].Rows[0];

            PrintData _pd = new PrintData()
            {
                page_width = (int)getCellValue(dr, "page_width", typeof(int)),
                divider_char_header = (string)getCellValue(dr, "divider_char_header", typeof(string)),
                divider_char_table = (string)getCellValue(dr, "divider_char_table", typeof(string)),
                rhead_main1 = (string)getCellValue(dr, "rhead_main1", typeof(string)),
                rhead_sub1 = (string)getCellValue(dr, "rhead_sub1", typeof(string)),
                rhead_sub2 = (string)getCellValue(dr, "rhead_sub2", typeof(string)),
                rhead_sub3 = (string)getCellValue(dr, "rhead_sub3", typeof(string)),
                rhead_inner1 = (string)getCellValue(dr, "rhead_inner1", typeof(string)),

                rhead_left_width = (int)getCellValue(dr, "rhead_left_width", typeof(int)),
                rhead_text_left = (string)getCellValue(dr, "rhead_text_left", typeof(string)),
                rhead_text_right = (string)getCellValue(dr, "rhead_text_right", typeof(string)),
                rhead_text = (string)getCellValue(dr, "rhead_text", typeof(string)),

                rfoot_left_width = (int)getCellValue(dr, "rfoot_left_width", typeof(int)),
                rfoot_text_left = (string)getCellValue(dr, "rfoot_text_left", typeof(string)),
                rfoot_text_right = (string)getCellValue(dr, "rfoot_text_right", typeof(string)),
                rfoot_text = (string)getCellValue(dr, "rfoot_text", typeof(string))
            };

            if (ds.Tables.Count >= 3)
                GetPrintData_inner(_pd.table1_meta, _pd.table1, ds.Tables[1], ds.Tables[2]);
            if (ds.Tables.Count >= 5)
                GetPrintData_inner(_pd.table2_meta, _pd.table2, ds.Tables[3], ds.Tables[4]);
            if (ds.Tables.Count >= 7)
                GetPrintData_inner(_pd.table3_meta, _pd.table3, ds.Tables[3], ds.Tables[4]);

            return _pd;
        }

        private void GetPrintData_inner(PrintDataHeader meta, List<PrintDataLines> table, EbDataTable dt1, EbDataTable dt2)
        {
            EbDataRow dr = dt1.Rows[0];
            PrintDataLines lines;
            meta.dhead_text = (string)getCellValue(dr, "dhead_text", typeof(string));
            meta.dfoot_text = (string)getCellValue(dr, "dfoot_text", typeof(string));
            meta.th_left1 = (string)getCellValue(dr, "th_left1", typeof(string));
            meta.th_left2 = (string)getCellValue(dr, "th_left2", typeof(string));
            meta.th_left3 = (string)getCellValue(dr, "th_left3", typeof(string));
            meta.th_right1 = (string)getCellValue(dr, "th_right1", typeof(string));
            meta.th_right2 = (string)getCellValue(dr, "th_right2", typeof(string));
            meta.th_right3 = (string)getCellValue(dr, "th_right3", typeof(string));
            meta.th_right4 = (string)getCellValue(dr, "th_right4", typeof(string));
            meta.th_right5 = (string)getCellValue(dr, "th_right5", typeof(string));
            meta.th_right6 = (string)getCellValue(dr, "th_right6", typeof(string));
            meta.th_right7 = (string)getCellValue(dr, "th_right7", typeof(string));
            meta.th_right8 = (string)getCellValue(dr, "th_right8", typeof(string));

            foreach (EbDataRow row in dt2.Rows)
            {
                lines = new PrintDataLines();
                lines.td_left1 = (string)getCellValue(row, "td_left1", typeof(string));
                lines.td_left2 = (string)getCellValue(row, "td_left2", typeof(string));
                lines.td_left3 = (string)getCellValue(row, "td_left3", typeof(string));
                lines.td_right1 = (string)getCellValue(row, "td_right1", typeof(string));
                lines.td_right2 = (string)getCellValue(row, "td_right2", typeof(string));
                lines.td_right3 = (string)getCellValue(row, "td_right3", typeof(string));
                lines.td_right4 = (string)getCellValue(row, "td_right4", typeof(string));
                lines.td_right5 = (string)getCellValue(row, "td_right5", typeof(string));
                lines.td_right6 = (string)getCellValue(row, "td_right6", typeof(string));
                lines.td_right7 = (string)getCellValue(row, "td_right7", typeof(string));
                lines.td_right8 = (string)getCellValue(row, "td_right8", typeof(string));
                table.Add(lines);
            }
        }

        private async Task PrintPageSV_V2(PrintData _data)
        {
            if (_outputStream == null)
            {
                throw new Exception("Printer not connected.");
            }

            int NormalMaxChars = _data.page_width > 0 ? _data.page_width : 69;
            int LargeMaxChars = NormalMaxChars / 2;
            _data.rhead_left_width = _data.rhead_left_width > 0 ? _data.rhead_left_width : NormalMaxChars / 2;
            _data.divider_char_header = string.IsNullOrWhiteSpace(_data.divider_char_header) ? "*" : _data.divider_char_header;
            _data.divider_char_table = string.IsNullOrWhiteSpace(_data.divider_char_table) ? "-" : _data.divider_char_table;

            // ESC/POS Commands
            byte[] initCommand = new byte[] { 0x1B, 0x40 }; // ESC @ (Initialize Printer)
            byte[] rightAlign = new byte[] { 0x1B, 0x61, 0x02 }; // ESC a 2 (Right Align)
            byte[] centerAlign = new byte[] { 0x1B, 0x61, 0x01 }; // ESC a 1 (Center Align)
            byte[] leftAlign = new byte[] { 0x1B, 0x61, 0x00 }; // ESC a 0 (Left Align)
            byte[] boldOn = new byte[] { 0x1B, 0x45, 0x01 }; // Bold On
            byte[] boldOff = new byte[] { 0x1B, 0x45, 0x00 }; // Bold Off
            byte[] lineFeed = new byte[] { 0x0A }; // Line Feed
            byte[] fontNormal = new byte[] { 0x1B, 0x21, 0x00 }; // normal font
            byte[] fontdHeight = new byte[] { 0x1B, 0x21, 0x10 }; // Double height
            byte[] fontdWidth = new byte[] { 0x1B, 0x21, 0x20 }; // Double width
            byte[] fontLarge = new byte[] { 0x1B, 0x21, 0x30 }; // Double width & height (Large)
            byte[] setFont = { 27, 77, 0 };
            byte[] HEAD_DIVIDER = leftAlign.Concat(Encoding.UTF8.GetBytes(RepeatToLength(_data.divider_char_header, NormalMaxChars))).ToArray();
            byte[] DIVIDER = leftAlign.Concat(Encoding.UTF8.GetBytes(RepeatToLength(_data.divider_char_table, NormalMaxChars))).ToArray();

            List<byte> bytes = new List<byte>();
            bytes.AddRange(initCommand);
            //bytes.AddRange(setFont);

            //bytes.AddRange(lineFeed);
            bytes.AddRange(HEAD_DIVIDER);
            bytes.AddRange(lineFeed);

            bytes.AddRange(fontLarge);
            bytes.AddRange(centerAlign);
            if (!string.IsNullOrWhiteSpace(_data.rhead_main1))
            {
                bytes.AddRange(Encoding.UTF8.GetBytes(_data.rhead_main1));
                bytes.AddRange(lineFeed);
            }

            bytes.AddRange(fontNormal);
            bytes.AddRange(centerAlign);
            if (!string.IsNullOrWhiteSpace(_data.rhead_sub1))
            {
                bytes.AddRange(Encoding.UTF8.GetBytes(_data.rhead_sub1));
                bytes.AddRange(lineFeed);
            }

            if (!string.IsNullOrWhiteSpace(_data.rhead_sub2))
            {
                bytes.AddRange(Encoding.UTF8.GetBytes(_data.rhead_sub2));
                bytes.AddRange(lineFeed);
            }

            if (!string.IsNullOrWhiteSpace(_data.rhead_sub3))
            {
                bytes.AddRange(Encoding.UTF8.GetBytes(_data.rhead_sub3));
                bytes.AddRange(lineFeed);
            }

            bytes.AddRange(HEAD_DIVIDER);
            bytes.AddRange(lineFeed);

            bytes.AddRange(centerAlign);
            if (!string.IsNullOrWhiteSpace(_data.rhead_inner1))
            {
                bytes.AddRange(Encoding.UTF8.GetBytes(new string('-', _data.rhead_inner1.Length)));
                bytes.AddRange(lineFeed);
                bytes.AddRange(Encoding.UTF8.GetBytes(_data.rhead_inner1));
                bytes.AddRange(lineFeed);
                bytes.AddRange(Encoding.UTF8.GetBytes(new string('-', _data.rhead_inner1.Length)));
                bytes.AddRange(lineFeed);
            }
            bytes.AddRange(leftAlign);

            string temp;
            List<string> arr1 = _data.rhead_text_left.Split("$$$").ToList();
            List<string> arr2 = _data.rhead_text_right.Split("$$$").ToList();


            for (int i = 0; i < arr1.Count || i < arr2.Count; i++)
            {
                temp = string.Empty;

                if (i < arr1.Count)
                {
                    if (arr1[i].Length > _data.rhead_left_width)
                    {
                        arr1.Insert(i + 1, arr1[i].Substring(_data.rhead_left_width));
                        arr1[i] = arr1[i].Substring(0, _data.rhead_left_width);
                    }
                    temp += arr1[i].PadRight(_data.rhead_left_width);
                }
                if (i < arr2.Count)
                {
                    if (arr2[i].Length > NormalMaxChars - _data.rhead_left_width)
                    {
                        arr2.Insert(i + 1, arr2[i].Substring(NormalMaxChars - _data.rhead_left_width));
                        arr2[i] = arr2[i].Substring(0, NormalMaxChars - _data.rhead_left_width);
                    }
                    temp += arr2[i];
                }
                if (temp.Length > 0)
                {
                    bytes.AddRange(Encoding.UTF8.GetBytes(temp));
                    bytes.AddRange(lineFeed);
                }
            }

            if (!string.IsNullOrWhiteSpace(_data.rhead_text))
            {
                bytes.AddRange(Encoding.UTF8.GetBytes(_data.rhead_text));
                bytes.AddRange(lineFeed);
            }

            if (_data.table1_meta.th_left1?.Length > 0)
                PrintPageSV_V2_inner(bytes, _data.table1_meta, _data.table1, lineFeed, DIVIDER);
            if (_data.table2_meta.th_left1?.Length > 0)
                PrintPageSV_V2_inner(bytes, _data.table2_meta, _data.table2, lineFeed, DIVIDER);
            if (_data.table3_meta.th_left1?.Length > 0)
                PrintPageSV_V2_inner(bytes, _data.table3_meta, _data.table3, lineFeed, DIVIDER);

            arr1 = _data.rfoot_text_left.Split("$$$").ToList();
            arr2 = _data.rfoot_text_right.Split("$$$").ToList();

            for (int i = 0; i < arr1.Count || i < arr2.Count; i++)
            {
                temp = string.Empty;

                if (i < arr1.Count)
                {
                    if (arr1[i].Length > _data.rfoot_left_width)
                    {
                        arr1.Insert(i + 1, arr1[i].Substring(_data.rfoot_left_width));
                        arr1[i] = arr1[i].Substring(0, _data.rfoot_left_width);
                    }
                    temp += arr1[i].PadRight(_data.rfoot_left_width);
                }
                if (i < arr2.Count)
                {
                    if (arr2[i].Length > NormalMaxChars - _data.rfoot_left_width)
                    {
                        arr2.Insert(i + 1, arr2[i].Substring(NormalMaxChars - _data.rfoot_left_width));
                        arr2[i] = arr2[i].Substring(0, NormalMaxChars - _data.rfoot_left_width);
                    }
                    temp += arr2[i];
                }
                if (temp.Length > 0)
                {
                    bytes.AddRange(Encoding.UTF8.GetBytes(temp));
                    bytes.AddRange(lineFeed);
                }
            }

            if (!string.IsNullOrWhiteSpace(_data.rfoot_text))
            {
                bytes.AddRange(Encoding.UTF8.GetBytes(_data.rfoot_text));
                bytes.AddRange(lineFeed);
            }

            bytes.AddRange(lineFeed);
            bytes.AddRange(HEAD_DIVIDER);
            bytes.AddRange(lineFeed);
            bytes.AddRange(lineFeed);
            bytes.AddRange(lineFeed);
            bytes.AddRange(lineFeed);
            bytes.AddRange(lineFeed);

            // Cut Paper (if supported)
            byte[] cut = new byte[] { 0x1D, 0x56, 0x41, 0x00 }; // ESC/POS Cut Paper
            bytes.AddRange(cut);

            foreach (var chunk in ChunkData(bytes.ToArray(), 256))
            {
                await _outputStream.WriteAsync(chunk, 0, chunk.Length);
                await Task.Delay(200);
            }

            //await _outputStream.WriteAsync(bytes.ToArray(), 0, bytes.Count);
            await _outputStream.FlushAsync();
            await Task.Delay(3000);
        }

        private void PrintPageSV_V2_inner(List<byte> bytes, PrintDataHeader head, List<PrintDataLines> table, byte[] lineFeed, byte[] DIVIDER)
        {
            if (!string.IsNullOrWhiteSpace(head.dhead_text))
            {
                bytes.AddRange(Encoding.UTF8.GetBytes(head.dhead_text));
                bytes.AddRange(lineFeed);
            }

            bytes.AddRange(DIVIDER);
            bytes.AddRange(lineFeed);
            string header = head.th_left1 + head.th_left2 + head.th_left3 +
                head.th_right1 + head.th_right2 + head.th_right3 + head.th_right4 + head.th_right5 + head.th_right6 + head.th_right7 + head.th_right8;
            bytes.AddRange(Encoding.UTF8.GetBytes(header));
            //bytes.AddRange(boldOff);
            bytes.AddRange(lineFeed);
            bytes.AddRange(DIVIDER);
            bytes.AddRange(lineFeed);

            foreach (PrintDataLines item in table)
            {
                if (item.td_left1.Length > head.th_left1.Length)
                {
                    string tempstr = item.td_left1.Substring(0, head.th_left1.Length);
                    item.td_left1 = item.td_left1.Substring(head.th_left1.Length);
                    bytes.AddRange(Encoding.UTF8.GetBytes(tempstr));
                    bytes.AddRange(lineFeed);
                }
                string itemLine = item.td_left1.PadRight(head.th_left1.Length);
                if (head.th_left2.Length > 0)
                    itemLine += item.td_left2.PadRight(head.th_left2.Length);
                if (head.th_left3.Length > 0)
                    itemLine += item.td_left3.PadRight(head.th_left3.Length);
                if (head.th_right1.Length > 0)
                    itemLine += item.td_right1.PadLeft(head.th_right1.Length);
                if (head.th_right2.Length > 0)
                    itemLine += item.td_right2.PadLeft(head.th_right2.Length);
                if (head.th_right3.Length > 0)
                    itemLine += item.td_right3.PadLeft(head.th_right3.Length);
                if (head.th_right4.Length > 0)
                    itemLine += item.td_right4.PadLeft(head.th_right4.Length);
                if (head.th_right5.Length > 0)
                    itemLine += item.td_right5.PadLeft(head.th_right5.Length);
                if (head.th_right6.Length > 0)
                    itemLine += item.td_right6.PadLeft(head.th_right6.Length);
                if (head.th_right7.Length > 0)
                    itemLine += item.td_right7.PadLeft(head.th_right7.Length);
                if (head.th_right8.Length > 0)
                    itemLine += item.td_right8.PadLeft(head.th_right8.Length);

                bytes.AddRange(Encoding.UTF8.GetBytes(itemLine));
                bytes.AddRange(lineFeed);
            }

            bytes.AddRange(DIVIDER);
            bytes.AddRange(lineFeed);

            if (!string.IsNullOrWhiteSpace(head.dfoot_text))
            {
                bytes.AddRange(Encoding.UTF8.GetBytes(head.dfoot_text));
                bytes.AddRange(lineFeed);
            }
        }

        private async Task PrintPageSV_V1(InvoiceData _data)
        {
            if (_outputStream == null)
            {
                throw new Exception("Printer not connected.");
            }

            int NormalMaxChars = 69;
            int LargeMaxChars = 34;
            // ESC/POS Commands
            byte[] initCommand = new byte[] { 0x1B, 0x40 }; // ESC @ (Initialize Printer)
            byte[] rightAlign = new byte[] { 0x1B, 0x61, 0x02 }; // ESC a 2 (Right Align)
            byte[] centerAlign = new byte[] { 0x1B, 0x61, 0x01 }; // ESC a 1 (Center Align)
            byte[] leftAlign = new byte[] { 0x1B, 0x61, 0x00 }; // ESC a 0 (Left Align)
            byte[] boldOn = new byte[] { 0x1B, 0x45, 0x01 }; // Bold On
            byte[] boldOff = new byte[] { 0x1B, 0x45, 0x00 }; // Bold Off
            byte[] lineFeed = new byte[] { 0x0A }; // Line Feed
            byte[] fontNormal = new byte[] { 0x1B, 0x21, 0x00 }; // normal font
            byte[] fontdHeight = new byte[] { 0x1B, 0x21, 0x10 }; // Double height
            byte[] fontdWidth = new byte[] { 0x1B, 0x21, 0x20 }; // Double width
            byte[] fontLarge = new byte[] { 0x1B, 0x21, 0x30 }; // Double width & height (Large)
            byte[] setFont = { 27, 77, 0 };
            byte[] DIVIDER = leftAlign.Concat(fontdWidth.Concat(boldOn.Concat(Encoding.UTF8.GetBytes(new string('-', LargeMaxChars)).Concat(boldOff.Concat(fontNormal))))).ToArray();
            byte[] HEAD_DIVIDER = leftAlign.Concat(Encoding.UTF8.GetBytes(new string('*', NormalMaxChars))).ToArray();

            List<byte> bytes = new List<byte>();
            bytes.AddRange(initCommand);
            //bytes.AddRange(setFont);

            bytes.AddRange(lineFeed);
            bytes.AddRange(HEAD_DIVIDER);
            bytes.AddRange(lineFeed);

            bytes.AddRange(fontLarge);
            bytes.AddRange(centerAlign);
            bytes.AddRange(Encoding.UTF8.GetBytes(_data.store_name));
            bytes.AddRange(lineFeed);

            bytes.AddRange(fontNormal);
            bytes.AddRange(centerAlign);
            bytes.AddRange(Encoding.UTF8.GetBytes(_data.store_address));
            bytes.AddRange(lineFeed);

            if (!string.IsNullOrWhiteSpace(_data.store_vattrn))
            {
                bytes.AddRange(Encoding.UTF8.GetBytes(_data.store_vattrn));
                bytes.AddRange(lineFeed);
            }

            if (!string.IsNullOrWhiteSpace(_data.store_whatsapp))
            {
                bytes.AddRange(Encoding.UTF8.GetBytes(_data.store_whatsapp));
                bytes.AddRange(lineFeed);
            }

            bytes.AddRange(HEAD_DIVIDER);
            bytes.AddRange(lineFeed);

            bytes.AddRange(centerAlign);
            string temp = "|  TAX INVOICE  |";
            bytes.AddRange(Encoding.UTF8.GetBytes(new string('-', temp.Length)));
            bytes.AddRange(lineFeed);
            bytes.AddRange(Encoding.UTF8.GetBytes(temp));
            bytes.AddRange(lineFeed);
            bytes.AddRange(Encoding.UTF8.GetBytes(new string('-', temp.Length)));
            bytes.AddRange(lineFeed);
            bytes.AddRange(leftAlign);

            List<string> arr1 = _data.customer_details.Split("$$$").ToList();
            List<string> arr2 = _data.invoice_details.Split("$$$").ToList();

            for (int i = 0; i < arr1.Count || i < arr2.Count; i++)
            {
                temp = string.Empty;

                if (i < arr1.Count)
                {
                    if (arr1[i].Length > NormalMaxChars / 2)
                    {
                        arr1.Insert(i + 1, arr1[i].Substring(NormalMaxChars / 2));
                        arr1[i] = arr1[i].Substring(0, NormalMaxChars / 2);
                    }
                    temp += arr1[i].PadRight(NormalMaxChars / 2);
                }
                if (i < arr2.Count)
                {
                    if (arr2[i].Length > NormalMaxChars / 2)
                    {
                        arr2.Insert(i + 1, arr2[i].Substring(NormalMaxChars / 2));
                        arr2[i] = arr2[i].Substring(0, NormalMaxChars / 2);
                    }
                    temp += arr2[i];
                }
                if (temp.Length > 0)
                {
                    bytes.AddRange(Encoding.UTF8.GetBytes(temp));
                    bytes.AddRange(lineFeed);
                }
            }

            bytes.AddRange(DIVIDER);
            bytes.AddRange(lineFeed);

            // Print Table Header
            //bytes.AddRange(boldOn);
            string header = "ITEM NAME".PadRight(37) + "RATE".PadLeft(8) + "QTY+".PadLeft(6) + "RTN-".PadLeft(7) + "AMT".PadLeft(10);
            bytes.AddRange(Encoding.UTF8.GetBytes(header));
            //bytes.AddRange(boldOff);
            bytes.AddRange(lineFeed);
            bytes.AddRange(DIVIDER);
            bytes.AddRange(lineFeed);

            double totalAmt = 0;
            double totalVat = 0;
            double totalNetAmt = 0;
            foreach (InvoiceDataLines item in _data.sal_items)
            {
                if (item.item_name.Length > 38)
                {
                    string tempstr = item.item_name.Substring(0, 37);
                    item.item_name = item.item_name.Substring(37);
                    bytes.AddRange(Encoding.UTF8.GetBytes(tempstr));
                    bytes.AddRange(lineFeed);
                }
                string itemLine = item.item_name.PadRight(37) + $"{item.rate,8:0.00}{item.qty,6}" + item.rtn.PadLeft(7) + $"{item.net_amt,10:0.00}";
                bytes.AddRange(Encoding.UTF8.GetBytes(itemLine));
                bytes.AddRange(lineFeed);

                totalAmt += item.amt;
                totalVat += item.vat_amt;
                totalNetAmt += item.net_amt;
            }

            bytes.AddRange(DIVIDER);
            bytes.AddRange(lineFeed);

            // Print Total
            string totalLine = $"GROSS AMOUNT : AED{totalAmt,9:0.00}";//14
            string vatTotalLine = $"VAT 5% TOTAL : AED{totalVat,9:0.00}";//14
            string netTotalLine = $"NET AMOUNT   : AED{totalNetAmt,9:0.00}";//14

            string amtReceived = $"AMT RECEIVED      : AED{_data.amt_received,9:0.00}";//14
            string closingBalance = $"TOTAL OUTSTANDING : AED{_data.closing_balance,9:0.00}";//19

            bytes.AddRange(leftAlign);
            //bytes.AddRange(boldOn);
            totalLine = totalLine.PadLeft(NormalMaxChars - 2);
            bytes.AddRange(Encoding.UTF8.GetBytes(totalLine));
            bytes.AddRange(lineFeed);

            vatTotalLine = amtReceived.PadRight(NormalMaxChars - vatTotalLine.Length - 2) + vatTotalLine;
            bytes.AddRange(Encoding.UTF8.GetBytes(vatTotalLine));
            bytes.AddRange(lineFeed);

            netTotalLine = closingBalance.PadRight(NormalMaxChars - netTotalLine.Length - 2) + netTotalLine;
            bytes.AddRange(Encoding.UTF8.GetBytes(netTotalLine));
            bytes.AddRange(lineFeed);
            bytes.AddRange(lineFeed);
            bytes.AddRange(HEAD_DIVIDER);
            bytes.AddRange(lineFeed);
            bytes.AddRange(lineFeed);
            bytes.AddRange(lineFeed);
            bytes.AddRange(lineFeed);
            bytes.AddRange(lineFeed);

            // Cut Paper (if supported)
            byte[] cut = new byte[] { 0x1D, 0x56, 0x41, 0x00 }; // ESC/POS Cut Paper
            bytes.AddRange(cut);

            foreach (var chunk in ChunkData(bytes.ToArray(), 256))
            {
                await _outputStream.WriteAsync(chunk, 0, chunk.Length);
                await Task.Delay(200);
            }

            //await _outputStream.WriteAsync(bytes.ToArray(), 0, bytes.Count);
            await _outputStream.FlushAsync();
            await Task.Delay(3000);
        }

        IEnumerable<byte[]> ChunkData(byte[] data, int chunkSize)
        {
            for (int i = 0; i < data.Length; i += chunkSize)
                yield return data.Skip(i).Take(chunkSize).ToArray();
        }

        private static string RepeatToLength(string input, int totalLength)
        {
            if (string.IsNullOrEmpty(input) || totalLength <= 0)
                return string.Empty;

            var repeated = new StringBuilder();

            while (repeated.Length + input.Length <= totalLength)
            {
                repeated.Append(input);
            }

            int remaining = totalLength - repeated.Length;
            if (remaining > 0)
            {
                repeated.Append(input.Substring(0, remaining));
            }

            return repeated.ToString();
        }
    }
}