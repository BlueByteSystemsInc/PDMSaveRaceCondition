using BlueByte.SOLIDWORKS.Extensions;
using BlueByte.SOLIDWORKS.Extensions.Helpers;
using EPDM.Interop.epdm;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMSaveRaceCondition
{
    class Program
    {
        #region fields 

        static string vaultName = "bluebyte";
        static int handle = 0;
        static SOLIDWORKSInstanceManager.Year_e solidworksYear = SOLIDWORKSInstanceManager.Year_e.Latest;

        #endregion

        static void Main(string[] args)
        {
            // Please read the fields region and set the value according your env.

            handle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.ToInt32();

            var vault = new EdmVault5();

            vault.LoginAuto(vaultName, handle);

            var rootFolder = vault.RootFolder;

            var testFolder = rootFolder.CreateFolderPath("\\TestFolder",handle);

            var solidworksInstanceManager = new SOLIDWORKSInstanceManager();

            // timeout in 30 seconds if it cant get a new instance 
            var instance = solidworksInstanceManager.GetNewInstance("/r/b", solidworksYear);

            instance.Visible = true;

            instance.MaximizeWindow();

            var swModel = instance.NewPart() as ModelDoc2;

            var path = $"{testFolder.LocalPath}\\testpart.sldprt";

            swModel.SaveAs(path);

            instance.QuitDoc(path);

            // this line mimicks a race condition  between the auto pdm add feature (that adds the file) and the last line 
            System.Threading.Thread.Sleep(3000);

            testFolder.AddFile(handle, path);

        }
    }
}
