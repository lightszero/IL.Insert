using System;

namespace testconsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ILInsertTool.ILAssembly ass = new ILInsertTool.ILAssembly();


            //step 01 - 03 is for depoly 
            //load src dll
            //using (
            var ms = System.IO.File.OpenRead("testlib.dll");
            //)
            {
                //step 01 load srcdll
                ass.Load(ms);


                //step 02
                //insert calc price code.
                ass.InsertCalcCode();

                //step 03
                //write dest dll
                System.IO.File.Delete("testlib2.dll");

                using (var _ms = System.IO.File.OpenWrite("testlib2.dll"))
                {
                    ass.Save(_ms);
                }


            }
            //src func is:    
            //    static int Main(int a)
            //after insert price calc code id
            //    static int Main(int a,CalcTool price)

            //step 04 call it with price.
            var outdllpath = System.IO.Path.GetFullPath("testlib2.dll");
            var assembly = System.Reflection.Assembly.LoadFile(outdllpath);
            var type = assembly.GetType("testlib.Program");
            var method = type.GetMethod("Main");
            var price = new calctool.CalcTool();
            price.price = 500;
            Console.WriteLine("set max price=" + price.price);
            method.Invoke(null, new object[] { 5, price });
            Console.WriteLine("after run it price=" + price.price);
            Console.ReadLine();
        }

    }
}
