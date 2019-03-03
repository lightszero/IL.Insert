# a neo vm speed up solution

this is s  neo vm speed up solution.

run il in .dll directly instead of .avm

with this,we can change neo vm contract -> natice contract to got a  natice speed

## sample
step0 user write a contract

step1 node got user dll

step2 node insert calc code

step3 node save changed dll

step4 call contract with price calc

### src code (user wirte)
        public static int Main(int a)
        {
            var aa = 1;
            for(var i=0;i<6;i++)
            {
                aa += i;
            }
            return aa;
        }

in testlib.dll


### dest code (IL Processer auto insert )

int testlib2.dll
check code with ilspy

        private static int Main(int a, CalcTool price)
        {
	        int num = 1;
	        int num2 = 0;
	        price.Add(5);
	        while (true)
	        {
		        bool flag = num2 < 6;
		        bool num3 = flag;
		        price.Add(16);
		        if (!num3)
		        {
			        break;
		        }
		        num += num2;
		        num2++;
	        }
	        int num4 = num;
	        price.Add(3);
	        int result = num4;
	        price.Add(2);
	        return result;
        }

### calc price
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