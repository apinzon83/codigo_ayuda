using carga_sv.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace carga_sv
{
    class Program
    {
        static int totalArchivos = 0;

        static void Main(string[] args)
        { startReadTefFile(); }

        private static void startReadTefFile()
        {
            string msg = "Leer Archivo Remesas.";
            Console.WriteLine(msg);
            escribirLog(msg);

            //int enc_bsv = 82;
            //int enc_vrz = 84;
            //int enc_pjf = 104;
            //int long_enc = 0;

            DateTime inicio = DateTime.Now;
            DateTime fin;
            msg = "Iniciamos " + inicio;
            Console.WriteLine(msg);
            escribirLog(msg);

            Settings webConf = new Settings();
            string cc        = webConf.cc;
            string tt        = webConf.tabla;
            string ci        = webConf.carpetaIn;
            string co        = webConf.carpetaOut;
            string ccCargaId = webConf.ccCargaId;
            string cat_ext = webConf.cat_ext;
            bool cont_DirRet = webConf.cont_DirRet;
            totalArchivos    = 0;

            //string[] files   = Directory.GetFiles(ci, "*.BSV"); //long_enc = enc_bsv;
            //recorrer_archivos(files,  cc, tt, ccCargaId, co, cont_DirRet);
            //string[] files2  = Directory.GetFiles(ci, "*.VRZ"); //long_enc = enc_vrz;
            //recorrer_archivos(files2, cc, tt, ccCargaId, co, cont_DirRet);
            //string[] files3  = Directory.GetFiles(ci, "*.txt"); //long_enc = enc_pjf;
            //recorrer_archivos(files3, cc, tt, ccCargaId, co, cont_DirRet);
            //string[] files4  = Directory.GetFiles(ci, "*.CPA"); //long_enc = enc_pjf;
            //recorrer_archivos(files3, cc, tt, ccCargaId, co, cont_DirRet);

            string[] l_ext = cat_ext.Split(',');
            
            foreach (string ext in l_ext)
            {
                string[] files = Directory.GetFiles(ci, "*."+ext);
                msg = "Leyendo archivos *." + ext;
                Console.WriteLine(msg);
                escribirLog(msg);
                //foreach (string ff in files)
                //{
                //    msg = "Leyendo archivo: " + ff;
                //    Console.WriteLine(msg);
                //    escribirLog(msg);
                //}
                recorrer_archivos(files, cc, tt, ccCargaId, co, cont_DirRet);
            }

            fin = DateTime.Now;
            msg = "Terminamos " + fin;
            Console.WriteLine(msg);
            escribirLog(msg);
            msg = totalArchivos + " Archivos Leidos";
            Console.WriteLine(msg);
            escribirLog(msg);
            msg = " ";
            Console.WriteLine(msg);
            escribirLog(msg);
            //Console.WriteLine("Presione Enter para terminar...");
            //Console.ReadLine();
        }
        
        private static void recorrer_archivos(string[] files,string cc, string tt,string ccCargaId, string co, bool dir)
        {
            float  montoArchivo = 0;
            int    insertados   = 0;
            int    NOinsertados = 0;
            float  monto_acum   = 0;
            int    dir_mas_70   = 0;
            float  monto        = 0;
            int    regs         = 0;
            int    actual       = 0;
            string msg          = string.Empty;
            string empresa      = string.Empty;
            string archivo      = string.Empty;
            string s_regs       = string.Empty;
            string s_monto      = string.Empty;
            string Folio        = string.Empty;
            string Cons         = string.Empty;
            string NomBen       = string.Empty;
            string DirBen       = string.Empty;
            string TelBen       = string.Empty;
            string EdoBen       = string.Empty;
            string FecOrd       = string.Empty;
            string TipoPag      = string.Empty;
            string Cuenta       = string.Empty;
            string EntiId       = string.Empty;
            string SucuId       = string.Empty;
            string NomRet       = string.Empty;
            string DirRet       = string.Empty;
            string TelRet       = string.Empty;
            string EdoRet       = string.Empty;
            string Comision     = string.Empty;
            string ComTipCam    = string.Empty;
            string TipCam       = string.Empty;
            string FecVen       = string.Empty;
            
            foreach (string item in files)
            {
                msg = "Leemos " + Path.GetFileName(item);
                Console.WriteLine(msg);
                escribirLog(msg);
                montoArchivo = 0;
                insertados   = 0;
                NOinsertados = 0;
                monto_acum   = 0;
                dir_mas_70   = 0;
                bool insert  = false;

                Encoding encoding = Encoding.GetEncoding(1252);
                int linea = 0;
                foreach (string line in File.ReadLines(item, encoding))
                {
                    string line2 = line.Replace("'", "_");
                    line2        = line.Replace("/", "_");
                    int inicial  = 0;
                    switch (linea)
                    {
                        case 0:
                            // primea linea, ignorar
                            break;
                        case 1:
                            // encabezado
                            inicial  = buscar_caracter_inicial(line2);
                            empresa  = extraeDatosbyIni(line2, inicial);
                            inicial += empresa.Length + 11;
                            archivo  = extraeDatosbyIni(line2, inicial);
                            inicial += archivo.Length + 9;
                            s_regs   = extraeDatosbyIni(line2, inicial);
                            inicial += s_regs.Length + 9;
                            s_monto  = extraeDatosbyIni(line2, inicial);
                            int.TryParse(s_regs, out regs);
                            float.TryParse(s_monto, out montoArchivo);
                            break;
                        default:
                            // registros
                            if (line2.Length < 10)
                            {
                                // fin de archivo, ignorar
                            }
                            else
                            {
                                inicial  = buscar_caracter_inicial(line2);
                                Folio    = extraeDatosbyIni(line2, inicial);
                                inicial += Folio.Length + 8;
                                Cons     = extraeDatosbyIni(line2, inicial);
                                inicial += Cons.Length + 10;
                                NomBen   = extraeDatosbyIni(line2, inicial);
                                inicial += NomBen.Length + 10;
                                DirBen   = extraeDatosbyIni(line2, inicial);
                                inicial += DirBen.Length + 10;
                                TelBen   = extraeDatosbyIni(line2, inicial);
                                inicial += TelBen.Length + 10;
                                EdoBen   = extraeDatosbyIni(line2, inicial);
                                inicial += EdoBen.Length + 12;
                                s_monto  = extraeDatosbyIni(line2, inicial);
                                inicial += s_monto.Length + 10;
                                FecOrd   = extraeDatosbyIni(line2, inicial);
                                inicial += FecOrd.Length + 11;
                                TipoPag  = extraeDatosbyIni(line2, inicial);
                                inicial += TipoPag.Length + 10;
                                Cuenta   = extraeDatosbyIni(line2, inicial);
                                inicial += Cuenta.Length + 10;
                                EntiId   = extraeDatosbyIni(line2, inicial);
                                inicial += EntiId.Length + 10;
                                SucuId   = extraeDatosbyIni(line2, inicial);
                                inicial += SucuId.Length + 10;
                                NomRet   = extraeDatosbyIni(line2, inicial);
                                inicial += NomRet.Length + 10;
                                float.TryParse(s_monto, out monto);
                                if (dir)
                                {
                                    DirRet   = extraeDatosbyIni(line2, inicial);
                                    inicial += DirRet.Length + 10;
                                }
                                else { DirRet = string.Empty; }
                                TelRet    = extraeDatosbyIni(line2, inicial);
                                inicial  += TelRet.Length + 10;
                                EdoRet    = extraeDatosbyIni(line2, inicial);
                                inicial  += EdoRet.Length + 12;
                                Comision  = extraeDatosbyIni(line2, inicial);
                                inicial  += Comision.Length + 13;
                                ComTipCam = extraeDatosbyIni(line2, inicial);
                                inicial  += ComTipCam.Length + 10;
                                TipCam    = extraeDatosbyIni(line2, inicial);
                                inicial  += TipCam.Length + 10;
                                FecVen    = extraeDatosbyIni(line2, inicial);
                                if (DirBen.Length > 70)
                                {
                                    DirBen = DirBen.Substring(0, 70);
                                    dir_mas_70++;
                                    msg    = "Folio " + Folio + " con dirección con más 70 caractéres.";
                                    Console.WriteLine(msg);
                                    escribirLog(msg);
                                }
                                actual++;
                                insert = ejectuta_insert(cc, tt, empresa, Folio, EntiId, SucuId, TipoPag, FecOrd, monto, NomBen, DirBen,
                                                         TelBen, EdoBen, Cuenta, NomRet, DirRet, EdoRet, TelRet, FecVen, ComTipCam, ccCargaId, actual, regs);
                                if (insert)
                                { insertados++; monto_acum += monto; }
                                else { NOinsertados++; }
                            }
                            break;
                    }
                    linea++;
                }
                msg = insertados + " Registros insertados del archivo: " + Path.GetFileName(item);
                Console.WriteLine(msg);
                escribirLog(msg);
                if (NOinsertados > 0)
                {
                    msg = NOinsertados + " Registros NO insertados.";
                    Console.WriteLine(msg);
                    escribirLog(msg);
                }
                if (dir_mas_70>0)
                {
                    msg = dir_mas_70 + " Registros con dirección mayor a 70 caractéres.";
                    Console.WriteLine(msg);
                    escribirLog(msg);
                }
                msg = String.Format("{0:C} monto insertado:  ", monto_acum);
                Console.WriteLine(msg);
                escribirLog(msg);
                msg = String.Format("{0:C} monto en archivo: ", montoArchivo);
                Console.WriteLine(msg);
                escribirLog(msg);
                float montoBD = consultaMontoTotal(empresa);
                msg = String.Format("{0:C} monto en bd:      ", montoBD);
                Console.WriteLine(msg);
                escribirLog(msg);
                DateTime fin = DateTime.Now;
                msg = "Terminamos de leer " + Path.GetFileName(item);
                Console.WriteLine(msg);
                escribirLog(msg);
                string solonombre = Path.GetFileNameWithoutExtension(item);
                string soloext    = Path.GetExtension(item);
                string solofecha  = fin.ToString();
                solofecha         = solofecha.Replace('-', ' ');
                solofecha         = solofecha.Replace('/', ' ');
                solofecha         = solofecha.Replace(':', ' ');
                solofecha         = solofecha.Replace(' ', '_');
                string archSalida = co + solonombre + "_" + solofecha + soloext;
                File.Move(item, archSalida);
                msg               = "Movemos " + Path.GetFileName(item) + " a la carpeta de leidos.";
                Console.WriteLine(msg);
                escribirLog(msg);
                totalArchivos++;
                msg               = " ";
                Console.WriteLine(msg);
                escribirLog(msg);
            }   
        }

        private static bool ejectuta_insert(string cc,     string tt,     string empresa, string Folio,  string EntiId, string SucuId, string TipoPag, string FecOrd,    float  monto,     string NomBen, string DirBen, 
                                            string TelBen, string EdoBen, string Cuenta,  string NomRet, string DirRet, string EdoRet, string TelRet, string FecVen,  string ComTipCam, string ccCargaId, int actual,     int regs)
        {
            bool res    = false;
            string msg  = string.Empty;
            string qry  = "INSERT INTO " + tt + "(      RemeID     , RmopFolio     ,RmstID,      EntiID     ,     SucuID     ,     RmtmID      , RmopFechaOrden , RmopImporte , RmopNomBen     , RmopDirBen     , RmopTelBen     , RmopEdoBen     , RmopSucAdm,  RmopCtaAdm     , RmopFecReg,  RmopNomRet     , RmopDirRet     , RmopEdoRet     , RmopTelRet     , RmopFecVen     , RmopTipoCambio    , [RmopFecMov] ,[RmopAutoriza], [RmopIdentifica], [RmInArchID], [EmplID],    RmCCCargaID     , [EmplIDCC], [RmOpAutomatico], [EntiIDCC], [RmopFolioAlterno]) ";
                   qry += "               values ('" + empresa + "','" + Folio + "','0'   , '" + EntiId + "','" + SucuId + "','" + TipoPag + "','" + FecOrd + "'," + monto + ",'" + NomBen + "','" + DirBen + "','" + TelBen + "','" + EdoBen + "', '0000'    , '" + Cuenta + "', getdate() , '" + NomRet + "','" + DirRet + "','" + EdoRet + "','" + TelRet + "','" + FecVen + "','" + ComTipCam + "', '19000101'   ,' '           , ''              , '0'         , ' '     , '" + ccCargaId + "', ' '       , '0'             , '0000'    , '" + Folio + "'   ) ";
            using (SqlConnection sqlConnection = new SqlConnection(cc))
            {
                sqlConnection.Open();
                using (SqlCommand cmd = new SqlCommand(qry, sqlConnection))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                        res = true;
                        msg = actual + "/" + regs + " - Folio: " + Folio + " insertado.";
                    }
                    catch (Exception ex)
                    {
                        res = false;
                        msg = actual + "/" + regs + " - Folio: " + Folio + " NO insertado. \nError: " + ex.Message;
                    }
                }
                sqlConnection.Close();
            }
            Console.WriteLine(msg);
            escribirLog(msg);
            return res;
        }

        private static int buscar_caracter_inicial(string linea)
        {
            int res = 0;
            for (int i = 0; i < linea.Length; i++)
            {
                if (linea[i] == '"')
                { res = i+1; break; }
            }
            return res;
        }

        private static string extraeDatosbyIni(string linea, int inicio)
        {
            string res = string.Empty;
            for (int i = inicio; i < linea.Length; i++)
            {
                if (linea[i] == '"')
                { break; }
                else
                { res += linea[i]; }
            }
            return res;
        }

        private static float consultaMontoTotal(string empresa)
        {
            Settings webConf    = new Settings();
            string oCon         = webConf.cc;
            DataTable dtDato    = new DataTable();
            SqlConnection oConn = new SqlConnection(oCon);
            string s_total      = string.Empty;
            string qry          = "select sum(rmopimporte) from " + webConf.tabla + " where remeid = '" + empresa + "'";
            oConn.Open();
            SqlCommand cmd      = new SqlCommand(qry, oConn);
            SqlDataAdapter adap = new SqlDataAdapter(cmd);
            adap.Fill(dtDato);
            int countTable      = dtDato.Rows.Count;
            float total         = 0;
            if (countTable >= 1)
            {
                s_total = dtDato.Rows[0][0].ToString();
                float.TryParse(s_total, out total);
            }
            oConn.Close();
            return total;
        }

        public static void escribirLog(string txt)
        {
            Settings webConf = new Settings();
            string   URLlog  = webConf.carpetaLogs;
            DateTime ff      = DateTime.Now;
            string   archivo = URLlog + @"carga_dispersion_" + numConNdigitos(ff.Day.ToString(), 2) + numConNdigitos(ff.Month.ToString(), 2) + numConNdigitos(ff.Year.ToString(), 4) + ".txt";
            try
            {
                System.IO.StreamWriter file = new StreamWriter(archivo, true);
                file.WriteLine(ff.ToString() + "   --->   " + txt);
                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se escribio en log.");
                Console.WriteLine(ex.Message);
            }
        }

        public static string numConNdigitos(string texto, int dig)
        {
            while (texto.Length < dig)
            { texto = "0" + texto; }
            return texto;
        }
    }
}
