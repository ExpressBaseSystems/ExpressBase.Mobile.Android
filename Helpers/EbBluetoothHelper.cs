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
                App.Settings.SelectedBtDevice = Store.GetJSON<EbBTDevice>(AppConst.CURRENT_BT_PRINTER);

                if (App.Settings.SelectedBtDevice?.DeviceName == null)
                {
                    Utils.Toast("Please select a printer");
                    return false;
                }

                string query = layout.OfflineQuery.GetCode();
                List<Param> _p = JsonConvert.DeserializeObject<List<Param>>(param);
                EbDataSet ds = App.DataDB.DoQueries(query, _p.ToDbParams().ToArray());

                if (!(ds.Tables.Count == 2 && ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0))
                {
                    Utils.Toast("Failed to fetch data for printing");
                    return false;
                }

                EbDataRow dr = ds.Tables[0].Rows[0];

                InvoiceData _data = new InvoiceData()
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
                    _data.items.Add(new InvoiceDataLines()
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

                connected = await this.ConnectToPrinter(App.Settings.SelectedBtDevice.DeviceName);

                if (connected)
                {
                    await this.PrintPage(_data);

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
            public List<InvoiceDataLines> items { get; set; }

            public InvoiceData()
            {
                items = new List<InvoiceDataLines>();
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

        private async Task PrintPage(InvoiceData _data)
        {
            if (_outputStream == null)
            {
                throw new Exception("Printer not connected.");
            }

            try
            {
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

                bytes.AddRange(Encoding.UTF8.GetBytes(_data.store_vattrn));
                bytes.AddRange(lineFeed);

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
                foreach (InvoiceDataLines item in _data.items)
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
            catch (Exception ex)
            {
                Console.WriteLine("Print Error: " + ex.Message);
            }
        }

        IEnumerable<byte[]> ChunkData(byte[] data, int chunkSize)
        {
            for (int i = 0; i < data.Length; i += chunkSize)
                yield return data.Skip(i).Take(chunkSize).ToArray();
        }
    }
}