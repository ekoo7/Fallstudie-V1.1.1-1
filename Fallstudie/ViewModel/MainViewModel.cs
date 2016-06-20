using Fallstudie.DBModel;
using Fallstudie.Model;
using GalaSoft.MvvmLight.Command;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Windows.Storage.Pickers;
using System.Net.Http;
using System.Net.NetworkInformation;
using Windows.System.Threading;
using Windows.UI.Input.Inking;
using Windows.UI.Core;
using Fallstudie.ServiceReference1;
using Windows.Foundation;
using System.Threading;

namespace Fallstudie.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        const string COMPANY_DESC = "DreamHouse GmbH \nSchönbrunner Straße 10 / 1 \nA - 1120 Wien \noffice@DreamHouse.com";
        const string LOGIN_ERROR_MESSAGE = "Username oder Passwort ist falsch!";

        private string testService;
        public string TestService
        {
            get { return testService; }
            set { testService = value; OnChange("TestService"); }
        }
        AppSynchronisationServiceClient service = new AppSynchronisationServiceClient();
        public MainViewModel()
        {
            InitializeButtons();

            //Datenbank erstellen
            DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");
            if (!File.Exists(DbPath))
            {
                IsInternetConnected = NetworkInterface.GetIsNetworkAvailable();
                if (IsInternetConnected)
                {
                    SQLCreateTable();
                    SQLInsertAttributeGroup();
                    FirstSync();
                    ProjectsMethod();
                    //ManageAppointments();
                }
                else
                    FirstConnectionWithServer();
            }
            else
            {
                SQLResetTempTable();
                ProjectsMethod();
                //ManageAppointments();

                //DownloadImages();
            }
               ThreadPool.RunAsync(CheckNewImages);     
        }
        //Hier werden alle Buttons initialisiert
        private void InitializeButtons()
        {
            ButtonForwardChooseCustomer = new RelayCommand(ButtonForwardChooseCustomerMethod);
            ButtonForwardChooseHouse = new RelayCommand(ButtonForwardChooseHouseMethod);
            ButtonForwardChoosePlot = new RelayCommand(ButtonForwardChoosePlotMethod);
            ButtonForwardChooseWall = new RelayCommand(ButtonForwardChooseWallMethod);
            ButtonForwardChooseRoof = new RelayCommand(ButtonForwardChooseRoofMethod);
            ButtonForwardChooseWindowsDoors = new RelayCommand(ButtonForwardChooseWindowsDoorsMethod);
            ButtonForwardChooseEnergie = new RelayCommand(ButtonForwardChooseEnergieMethod);
            ButtonForwardChooseAddition = new RelayCommand(ButtonForwardChooseAdditionMethod);
            ButtonForwardChooseOutsideArea = new RelayCommand(ButtonForwardChooseOutsideAreaMethod);
            ButtonForwardSummary = new RelayCommand(ButtonForwardSummaryMethod);
            ButtonEditConfiguration = new RelayCommand(ButtonEditConfigurationMehtod);
            ButtonSaveConfiguration = new RelayCommand(ButtonSaveConfigurationMethod);
            ButtonCreateProject = new RelayCommand(ButtonCreateProjectMethod);
            ButtonUploadPlot = new RelayCommand(ButtonUploadPlotMethod);
            ButtonAddGroundPlotInfo = new RelayCommand(ButtonAddGroundPlotInfoMethod);
            ButtonLogout = new RelayCommand(LogOutMethod);
            ButtonCreatePdf = new RelayCommand(ButtonCreatePdfMethod);
            ButtonCreatePdfProjects = new RelayCommand(ButtonCreatePdfProjectsMethod);
            ButtonEditConfigurationCustomer = new RelayCommand(ButtonEditConfigurationCustomerMethod);
            ButtonCreateNewCustomer = new RelayCommand(ButtonCreateNewCustomerMethod);
            ButtonLogin = new RelayCommand(ButtonLoginMethod);
            ButtonSync = new RelayCommand(ButtonSyncMethod);
        }

        #region Synchro
        #region Properties
        public RelayCommand ButtonSync { get; set; }
        private bool isSynched = false;

        public bool IsSynched
        {
            get { return isSynched; }
            set { isSynched = value; }
        }

        #endregion
        #region Methods
        private async void FirstSync()
        {
            try
            {
                SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath);

                SQLSyncInsertUser(con);
                SQLSyncInsertAddress(con);
                SQLSyncInsertUserGroupMap(con);
                SQLSyncInsertUserGroups(con);
                
                SQLSyncInsertAppointmentStatus(con);
                SQLSyncInsertMessage(con);
                SQLSyncInsertAppointment(con);

                SQLSyncInsertHousePackageStatus(con);
                SQLSyncInsertHousePackage(con);
                SQLSyncInsertPackageAttribute(con);

                SQLSyncInsertAttribute(con);
                SQLSyncInsertHouseconfig(con);
                SQLSyncInsertHouseconfigHasAttribute(con);
                SQLSyncInsertHousefloor(con);
                SQLSyncInsertHousefloorPackage(con);

                SQLSyncInsertProject(con);
            }
            catch (Exception e)
            {
                var dialog = new MessageDialog(e.ToString());
                await dialog.ShowAsync();
            }
        }

        private async void ButtonSyncMethod()
        {
            try
            {
                MessageDialog msgDialog = new MessageDialog("Sind Sie sicher, dass Sie Ihre App synchronisieren möchten?");
                UICommand yesCmd = new UICommand("Ja");
                msgDialog.Commands.Add(yesCmd);
                UICommand noCmd = new UICommand("Nein");
                msgDialog.Commands.Add(noCmd);
                IUICommand cmd = await msgDialog.ShowAsync();
                if (cmd == yesCmd)
                {
                    SQLSyncSendTable();
                    if (isSynched == true)
                    {
                        SQLSyncDropTable();
                        FirstSync();
                    }
                }     
            }
            catch (Exception e)
            {
                var dialog = new MessageDialog(e.ToString());
                await dialog.ShowAsync();
            }
        }

        #endregion
        #region SQL
        private void SQLSyncDropTable()
        {
            List<int> ListA = new List<int>();
            List<int> ListH = new List<int>();
            List<int> ListHHA = new List<int>();
            List<int> ListHF = new List<int>();
            List<int> ListP = new List<int>();
            List<int> ListAP = new List<int>(); 
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    ListA = (from x in con.Table<DBModel.Attribute>()
                            select x.attribute_id).ToList();
                    foreach (var item in ListA)
                        con.Delete<DBModel.Attribute>(item);
                    ListH = (from x in con.Table<Houseconfig>()
                             select x.houseconfig_id).ToList();
                    foreach (var item in ListH)
                        con.Delete<Houseconfig>(item);
                    ListHHA = (from x in con.Table<Houseconfig_Has_Attribute>()
                             select x.houseconfig_id).ToList();
                    foreach (var item in ListHHA)
                        con.Delete<Houseconfig_Has_Attribute>(item);
                    ListHF = (from x in con.Table<Housefloor>()
                             select x.housefloor_id).ToList();
                    foreach (var item in ListHF)
                        con.Delete<Housefloor>(item);
                    ListP = (from x in con.Table<Project>()
                              select x.project_id).ToList();
                    foreach (var item in ListP)
                        con.Delete<Project>(item);
                    ListAP = (from x in con.Table<Ymdh_Appointment>()
                              select x.appointment_id).ToList();
                    foreach (var item in ListAP)
                        con.Delete<Ymdh_Appointment>(item);
                    con.Close();
                } 
            }
            catch (Exception)
            {
            }
        }

        private async void SQLSyncInsertAttribute(SQLiteConnection con)
        {
            List<Attributes> List = new List<Attributes>();
            List = await service.Get_attribute_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new DBModel.Attribute
                {
                    attribute_id = item.Attribute_ID,
                    description = item.Description,
                    price = item.Price,
                    image = item.Image,
                    deleted = item.Deleted,
                    attribute_group_id = item.Attribute_Group_ID
                });
            }
        }
        private async void SQLSyncInsertHouseconfig(SQLiteConnection con)
        {
            List<Houseconfig1> List = new List<Houseconfig1>();
            List = await service.Get_houseconfig_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new Houseconfig
                {
                    houseconfig_id = item.Houseconfig_ID,
                    price = item.Price,
                    status = item.Status,
                    price_floor = item.Price_Floor,
                    modifieddate = ConvertDateTime((DateTime)item.Modifieddate),
                    house_package_id = item.House_Package_ID,
                    consultant_user_id = item.Consultant_User_ID,
                    customer_user_id = item.Customer_User_ID,
                });
            }
        }
        private async void SQLSyncInsertHouseconfigHasAttribute(SQLiteConnection con)
        {
            List<HouseconfigHasAttribute> List = new List<HouseconfigHasAttribute>();
            List = await service.Get_houseconfig_has_attribute_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new Houseconfig_Has_Attribute
                {
                    houseconfig_id = item.houseconfig_id,
                    attribute_id = item.attribute_id,
                    amount = item.amount,
                    special = item.special
                });
            }
        }
        private async void SQLSyncInsertHousefloor(SQLiteConnection con)
        {
            List<HouseFloors> List = new List<HouseFloors>();
            List = await service.Get_housefloor_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new Housefloor
                {
                    housefloor_id = item.housefloor_id,
                    price = item.price,
                    sketch = item.sketch,
                    houseconfig_id = item.houseconfig_id,
                    area = item.area
                });
            }
        }
        private async void SQLSyncInsertHousefloorPackage(SQLiteConnection con)
        {
            List<HouseFloorPackage> List = new List<HouseFloorPackage>();
            List = await service.Get_housefloor_package_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new Housefloor_Package
                {
                    housefloor_id = item.housefloor_id,
                    price = item.price,
                    sketch = item.sketch,
                    house_package_id = item.house_package_id,
                    area = item.area
                });
            }
        }
        private async void SQLSyncInsertUser(SQLiteConnection con)
        {
            List<Users> List = new List<Users>();
            try
            {
                List = await service.Get_mdh_users_dataAsync();
                foreach (var item in List)
                {
                    con.InsertOrReplace(new Mdh_Users
                    {
                        id = (int)item.ID,
                        name = item.Name,
                        username = item.Username,
                        email = item.Email,
                        password = item.Password
                    });
                }
            }
            catch (Exception e)
            {
                var dialog = new MessageDialog(e.ToString());
                await dialog.ShowAsync();
            }
        }
        private async void SQLSyncInsertUserGroupMap(SQLiteConnection con)
        {
            List<UserGroupMap> List = new List<UserGroupMap>();
            List = await service.Get_mdh_user_usergroup_map_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new Mdh_User_Usergroup_Map
                {
                    user_id = (int)item.user_id,
                    group_id = (int)item.group_id
                });
            }
        }
        private async void SQLSyncInsertUserGroups(SQLiteConnection con)
        {
            List<UserGroups> List = new List<UserGroups>();
            List = await service.Get_mdh_usergroups_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new Mdh_Usergroups
                {
                    id = (int)item.id,
                    parent_id = (int)item.parent_id,
                    lft = item.lft,
                    rgt = item.rgt,
                    title = item.title,

                });
            }  
        }
        private async void SQLSyncInsertPackageAttribute(SQLiteConnection con)
        {
            List<PackageAttribute> List = new List<PackageAttribute>();
            List = await service.Get_house_package_has_attribute_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new Package_Not_Attribute
                {
                    house_package_id = item.house_package_id,
                    attribute_id = item.attribute_id
                });
            }
        }
        private async void SQLSyncInsertProject(SQLiteConnection con)
        {
            List<ProjectSync> List = new List<ProjectSync>();
            List = await service.Get_project_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new Project
                {
                    project_id = item.project_id,
                    startdate = item.startdate,
                    enddate = item.enddate,
                    invoice = item.invoice,
                    status = item.status,
                    description = item.description,
                    modifieddate = ConvertDateTime(item.modifieddate),
                    houseconfig_id = item.houseconfig_id,
                    customer_user_id = item.customer_user_id
                });
            }
        }
        private async void SQLSyncInsertAddress(SQLiteConnection con)
        {  
            List<Address> List = new List<Address>();
            List = await service.Get_ymdh_address_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new Ymdh_Address
                {
                    mdh_address_id = item.mdh_address_id,
                    ZIP = item.ZIP,
                    City = item.City,
                    Street = item.Street,
                    houseno = item.houseno,
                    country = item.country
                });
            }  
        }
        private async void SQLSyncInsertAppointment(SQLiteConnection con)
        {
            List<Appointments> List = new List<Appointments>();
            List = await service.Get_ymdh_appointment_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new Ymdh_Appointment
                {
                    appointment_id = item.Appointment_ID,
                    from_ = ConvertDateTime((DateTime)item.From),
                    appointment_status_id = item.Appointment_Status_ID,
                    house_package_id = item.House_Package_ID,
                    consultant_user_id = item.Consultant_User_ID,
                    user_id = item.User_ID,
                    message_id = item.Message_ID
                });
            }
        }
        private async void SQLSyncInsertAppointmentStatus(SQLiteConnection con)
        {
            List<AppointmentStatus> List = new List<AppointmentStatus>();
            List = await service.Get_ymdh_appointment_status_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new Ymdh_Appointment_Status
                {
                    appointment_status_id = item.Appointment_Status_ID,
                    description = item.Description
                });
            }
        }
        private async void SQLSyncInsertHousePackage(SQLiteConnection con)
        {
            List<HousePackage> List = new List<HousePackage>();
            List = await service.Get_ymdh_house_package_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new Ymdh_House_Package
                {
                    house_package_id = item.House_Package_ID,
                    description = item.Description,
                    image = item.Image,
                    price = item.Price,
                    house_package_status = item.House_Package_Status_ID,
                    producer_id = item.Producer_ID,
                    address_id = item.Address_ID,
                    housefloors = item.Housefloors
                });
            }
        }
        private async void SQLSyncInsertHousePackageStatus(SQLiteConnection con)
        {
            List<HousePackageStatus> List = new List<HousePackageStatus>();
            List = await service.Get_ymdh_house_package_status_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new Ymdh_House_Package_Status
                {
                    house_package_status_id = item.House_Package_Status,
                    description = item.Description
                });
            }
        }
        private async void SQLSyncInsertMessage(SQLiteConnection con)
        {
            List<ymdh_message> List = new List<ymdh_message>();
            List = await service.Get_ymdh_message_dataAsync();
            foreach (var item in List)
            {
                con.InsertOrReplace(new Ymdh_Message
                {
                    message_id = item.message_id,
                    summary = item.summary,
                    message = item.message,
                    message_date = ConvertDateTime((DateTime)item.message_date),
                    message_type = item.message_type,
                    user_id = item.user_id
                });
            }
        }

        private async void SQLSyncSendTable()
        {
            try
            {
                List<Mdh_Change> query = new List<Mdh_Change>();
                List<houseconfig> house = new List<houseconfig>();
                List<housefloor> houseFloor = new List<housefloor>();
                List<houseconfig_has_attribute> houseAttribute = new List<houseconfig_has_attribute>();
                List<project> listProject = new List<project>();
                List<attribute> listAttribute = new List<attribute>();
                List<ymdh_appointment> listAppointment = new List<ymdh_appointment>();

                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    query = (from a in con.Table<Mdh_Change>()
                             where a.synced == 0
                             select a).ToList();

                    foreach (var item in query)
                    {
                        if (item.ctable == "houseconfig")
                        {
                            var h = (from x in con.Table<Houseconfig>()
                                     where x.houseconfig_id == item.id
                                     select x).Single();
                            house.Add(new houseconfig
                            {
                                houseconfig_id = h.houseconfig_id,
                                price = h.price,
                                status = h.status,
                                price_floor = h.price_floor,
                                modifieddate = DateTime.Parse(h.modifieddate),
                                house_package_id = h.house_package_id,
                                consultant_user_id = h.consultant_user_id,
                                customer_user_id = h.customer_user_id,
                            });
                            con.Update(new Mdh_Change
                            {
                                cid = item.cid,
                                ctable = item.ctable,
                                id = item.id,
                                id2 = item.id2,
                                change = item.change,
                                dt = item.dt,
                                synced = 1
                            });
                        }
                        if (item.ctable == "housefloor")
                        {
                            var h = (from x in con.Table<Housefloor>()
                                     where x.housefloor_id == item.id
                                     select x).Single();
                            houseFloor.Add(new housefloor
                            {
                                housefloor_id = h.housefloor_id,
                                price = h.price,
                                sketch = h.sketch,
                                houseconfig_id = h.houseconfig_id,
                                area = h.area
                            });
                            con.Update(new Mdh_Change
                            {
                                cid = item.cid,
                                ctable = item.ctable,
                                id = item.id,
                                id2 = item.id2,
                                change = item.change,
                                dt = item.dt,
                                synced = 1
                            });
                        }
                        if (item.ctable == "houseconfig_has_attribute")
                        {
                            var h = (from x in con.Table<Houseconfig_Has_Attribute>()
                                     where x.houseconfig_id == item.id
                                     && x.attribute_id == item.id2
                                     select x).Single();
                            houseAttribute.Add(new houseconfig_has_attribute
                            {
                                houseconfig_id = h.houseconfig_id,
                                attribute_id = h.attribute_id,
                                amount = h.amount,
                                special = h.special
                            });
                            con.Update(new Mdh_Change
                            {
                                cid = item.cid,
                                ctable = item.ctable,
                                id = item.id,
                                id2 = item.id2,
                                change = item.change,
                                dt = item.dt,
                                synced = 1
                            });
                        }
                        if (item.ctable == "project")
                        {
                            var h = (from x in con.Table<Project>()
                                     where x.project_id == item.id
                                     select x).Single();
                            listProject.Add(new project
                            {
                                project_id = h.project_id,
                                startdate = h.startdate,
                                enddate = h.enddate,
                                invoice = h.invoice,
                                status = h.status,
                                description = h.description,
                                modifieddate = DateTime.Parse(h.modifieddate),
                                houseconfig_id = h.houseconfig_id,
                                customer_user_id = h.customer_user_id
                            });
                            con.Update(new Mdh_Change
                            {
                                cid = item.cid,
                                ctable = item.ctable,
                                id = item.id,
                                id2 = item.id2,
                                change = item.change,
                                dt = item.dt,
                                synced = 1
                            });
                        }
                        if (item.ctable == "attribute")
                        {
                            var h = (from x in con.Table<DBModel.Attribute>()
                                     where x.attribute_id == item.id
                                     select x).Single();
                            listAttribute.Add(new attribute
                            {
                                attribute_id = h.attribute_id,
                                description = h.description,
                                price = h.price,
                                image = h.image,
                                deleted = h.deleted,
                                attribute_group_id = h.attribute_group_id
                            });
                            con.Update(new Mdh_Change
                            {
                                cid = item.cid,
                                ctable = item.ctable,
                                id = item.id,
                                id2 = item.id2,
                                change = item.change,
                                dt = item.dt,
                                synced = 1
                            });
                        }
                        if (item.ctable == "ymdh_appointment")
                        {
                            var h = (from x in con.Table<Ymdh_Appointment>()
                                     where x.appointment_id == item.id
                                     select x).Single();
                            listAppointment.Add(new ymdh_appointment
                            {
                                appointment_id = h.appointment_id,
                                from_ = DateTime.Parse(h.from_),
                                appointment_status_id = h.appointment_status_id,
                                house_package_id = h.house_package_id,
                                consultant_user_id = h.consultant_user_id,
                                user_id = h.user_id,
                                message_id = h.message_id
                            });
                            con.Update(new Mdh_Change
                            {
                                cid = item.cid,
                                ctable = item.ctable,
                                id = item.id,
                                id2 = item.id2,
                                change = item.change,
                                dt = item.dt,
                                synced = 1
                            });
                        }
                    }
                    con.Close();
                    if (listAttribute.Count != 0 || house.Count != 0 || houseFloor.Count != 0 || houseAttribute.Count != 0 || listProject.Count != 0 || listAppointment.Count != 0)
                    {
                        if (listAttribute.Count != 0)
                            await service.Import_attributeAsync(listAttribute);
                        if (house.Count != 0)
                            await service.Import_houseconfigAsync(house);
                        if (houseFloor.Count != 0)
                            await service.Import_housefloorAsync(houseFloor);
                        if (houseAttribute.Count != 0)
                            await service.Import_houseconfig_has_attributeAsync(houseAttribute);
                        if (listProject.Count != 0)
                            await service.Import_projectAsync(listProject);
                        if (listAppointment.Count != 0)
                            await service.Import_ymdh_appointmentAsync(listAppointment);
                        isSynched = true;
                    }
                    else
                        isSynched = false;
                    var dialog = new MessageDialog("Die App wurde erfolgreich synchronisiert");
                    await dialog.ShowAsync();
                }  
            }
            catch (Exception e)
            {
                var dialog = new MessageDialog(e.ToString());
                await dialog.ShowAsync();
            }
        }

        #endregion

        #endregion

        #region Offline Funktionalität

        public StorageFolder HouseconfigFolder { get; set; }
        public bool IsInternetConnected { get; set; }
        public bool BoolNewImages { get; set; }

        public async void FirstConnectionWithServer()
        {
            MessageDialog msgDialog = new MessageDialog("Sie benötigen Internet beim erstmaligem Start der App.", "Fehler: Verbindung fehlgeschlagen!");
            UICommand yesCmd = new UICommand("Nochmal");
            msgDialog.Commands.Add(yesCmd);
            UICommand noCmd = new UICommand("Abbrechen");
            msgDialog.Commands.Add(noCmd);
            IUICommand cmd = await msgDialog.ShowAsync();
            if (cmd == yesCmd)
                if (!CheckInternetConnection())
                    FirstConnectionWithServer();
                else
                {
                    DownloadImages();
                    FirstSync();
                }
            else
                App.Current.Exit();
        }

        public bool CheckInternetConnection()
        {
            IsInternetConnected = NetworkInterface.GetIsNetworkAvailable();
            if (IsInternetConnected)
                return true;
            else
                return false;
        }

        public async void CheckNewImages(object abc)
        {
            while (true)
            {
                List<string> hp;
                List<string> attribute;
                List<string> housefloor;
                List<List<string>> test = new List<List<string>>();
                try
                {
                    using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                    {
                        hp = (from a in con.Table<Ymdh_House_Package>()
                              select a.image).ToList();
                        attribute = (from b in con.Table<DBModel.Attribute>()
                                     where !b.image.Equals("NULL")
                                     || !b.image.Equals(null)
                                     select b.image).ToList();
                        housefloor = (from c in con.Table<Housefloor_Package>()
                                      select c.sketch).ToList();
                        test.Add(hp);
                        test.Add(attribute);
                        test.Add(housefloor);
                        con.Close();
                    }
                    foreach (var item1 in test)
                    {
                        foreach (var item in item1)
                        {
                            string[] names = item.Substring(71).Split('/');
                            var rootFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DreamHouse\\Houseconfig\\" + names[0], CreationCollisionOption.OpenIfExists); // Create folder
                            string imagePath = rootFolder.Path + "\\" + names[1];
                            if (!File.Exists(imagePath))
                            {
                                if (CheckInternetConnection())
                                    DownloadImages();
                            }
                            else
                            {
                                byte[] size = File.ReadAllBytes(imagePath);
                                if (size.Length == 0)
                                    File.Delete(imagePath);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    
                }
                await Task.Delay(15000);
            }
        }

        public async void DownloadImages()
        {
            List<string> hp;
            List<string> attribute;
            List<string> housefloor;
            List<List<string>> test = new List<List<string>>();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    hp = (from a in con.Table<Ymdh_House_Package>()
                          select a.image).ToList();
                    attribute = (from b in con.Table<DBModel.Attribute>()
                                 where !b.image.Equals("NULL")
                                 || !b.image.Equals(null)
                                 select b.image).ToList();
                    housefloor = (from c in con.Table<Housefloor_Package>()
                                  select c.sketch).ToList();
                    for (int i = 0; i < hp.Count; i++)
                    {
                        hp[i] = "http://wi-gate.technikum-wien.at:60333/Joomla_3.3.6/images/houseconfig/2Haeuser/" + hp[i];
                    }
                    test.Add(hp);
                    test.Add(attribute);
                    test.Add(housefloor);
                    con.Close();
                }
                foreach (var item1 in test)
                {
                    foreach (var item in item1)
                    {
                        try
                        {
                            string[] names = item.Substring(71).Split('/');
                            var rootFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DreamHouse\\Houseconfig\\" + names[0], CreationCollisionOption.OpenIfExists); // Create folder
                            HouseconfigFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DreamHouse\\Houseconfig\\", CreationCollisionOption.OpenIfExists); // Create folder

                            var coverpic_file = await rootFolder.CreateFileAsync(names[1], CreationCollisionOption.FailIfExists); // Create file

                            HttpClient client = new HttpClient(); // Create HttpClient
                            byte[] buffer = await client.GetByteArrayAsync(item); // Download file
                            using (Stream stream = await coverpic_file.OpenStreamForWriteAsync())
                            {
                                stream.Write(buffer, 0, buffer.Length); // Save
                            }
                            using(SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                            {
                                if (item1 == hp)
                                {
                                    string a = names[1];
                                    Ymdh_House_Package c1 = (from c in con.Table<Ymdh_House_Package>()
                                                             where c.image == a
                                                             select c).Single();
                                    con.Update(new Ymdh_House_Package()
                                    {
                                        house_package_id = c1.house_package_id,
                                        description = c1.description,
                                        image = c1.image,
                                        price = c1.price,
                                        house_package_status = c1.house_package_status,
                                        producer_id = c1.producer_id,
                                        address_id = c1.address_id,
                                        housefloors = c1.housefloors,
                                        rootfolder = rootFolder.Path + "\\" + names[1]
                                    });
                                }
                                if (item1 == attribute)
                                {
                                    DBModel.Attribute c1 = (from c in con.Table<DBModel.Attribute>()
                                                            where c.image.Equals(item)
                                                            select c).Single();
                                    con.Update(new DBModel.Attribute()
                                    {
                                        attribute_id = c1.attribute_id,
                                        description = c1.description,
                                        price = c1.price,
                                        image = c1.image,
                                        deleted = c1.deleted,
                                        modifieddate = c1.modifieddate,
                                        attribute_group_id = c1.attribute_group_id,
                                        rootfolder = rootFolder.Path + "\\" + names[1]
                                    });
                                }
                                if (item1 == housefloor)
                                {
                                    Housefloor_Package c1 = (from c in con.Table<Housefloor_Package>()
                                                             where c.sketch.Equals(item)
                                                             select c).Single();
                                    con.Update(new Housefloor_Package()
                                    {
                                        housefloor_id = c1.housefloor_id,
                                        price = c1.price,
                                        sketch = c1.sketch,
                                        modifieddate = c1.modifieddate,
                                        house_package_id = c1.house_package_id,
                                        area = c1.area,
                                        rootfolder = rootFolder.Path + "\\" + names[1]
                                    });
                                }
                                con.Close();
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region Login

        public RelayCommand ButtonLogin { get; set; }
        public RelayCommand ButtonLogout { get; set; }
        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; OnChange("Username"); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; OnChange("Password"); }
        }
        private string loginErrorMessage;

        public string LoginErrorMessage
        {
            get { return loginErrorMessage;}
            set { loginErrorMessage = value; OnChange("LoginErrorMessage"); }
        }

        public void ButtonLoginMethod()
        {
            Customers.Clear();
            ListCustomer.Clear();
            SQLGetCustomers();
            DownloadImages();
            PasswordBox pwd = Login.GetPWD.GetString();

            List<Mdh_Users> model;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    model = (from a in con.Table<Mdh_Users>()
                             from b in con.Table<Mdh_User_Usergroup_Map>()
                             from c in con.Table<Mdh_Usergroups>()
                             where a.id.Equals(b.user_id)
                             && b.group_id.Equals(c.id)
                             && c.title.Equals("Consultant")
                             select a).ToList();
                    for (int i = 0; i < model.Count; i++)
                    {
                        if(model[i].username == Username && model[i].password == pwd.Password)
                        {
                            Username = model[i].username;
                            UserAppointments.Clear();
                            LoadUserAppointments();
                            SelectedConsultant = new Consultant() { Name = model[i].name, Id = model[i].id, Username = model[i].username};
                            a = StartPage.FrameObject.GetObject();
                            a.Navigate(typeof(MainPage));
                            break;
                        } 
                    }
                    con.Close();
                }
                LoginErrorMessage = LOGIN_ERROR_MESSAGE;
            }
            catch (Exception)
            {

            } 
        }

        //TODO: Alle Listen und Variablen zurücksetzen
        private async void LogOutMethod()
        {
            MessageDialog msgDialog = new MessageDialog("Wollen Sie sich ausloggen?", "Ausloggen.");
            UICommand yesCmd = new UICommand("Ja");
            msgDialog.Commands.Add(yesCmd);
            UICommand noCmd = new UICommand("Nein");
            msgDialog.Commands.Add(noCmd);
            IUICommand cmd = await msgDialog.ShowAsync();
            if (cmd == yesCmd)
            {
                // Launch the retrieved file
                StartPage.FrameObject.GetObject().Navigate(typeof(Login));
                LoginErrorMessage = "";

                //ES MÜSSEN NOCH ALLE VARIABLEN ZURÜCKGESETZ WERDEN
            }
        }
        #endregion

        #region Startseite
        private ObservableCollection<Appointment> userAppointments = new ObservableCollection<Appointment>();

        public ObservableCollection<Appointment> UserAppointments
        {
            get { return userAppointments; }
            set { userAppointments = value; }
        }

        private Appointment selectedUserAppointment;

        public Appointment SelectedUserAppointment
        {
            get { return selectedUserAppointment; }
            set {
                selectedUserAppointment = value;
                OnChange("SelectedUserAppointment");
                ButtonConfHouseForCustomerMethod();
            }
        }

        private void LoadUserAppointments()
        {   
            if (Appointments != null)
            {
                foreach (var item in Appointments)
                {
                    if (item.Consultant.Username == Username
                        && ((item.Date.Month == DateTimeNow.Month && item.Date.Day >= dateTimeNow.Day)  || item.Date.Month == DateTimeNow.Month + 1))
                    {
                        UserAppointments.Add(item);    
                    }      
                }
            }
        }

        private void ButtonConfHouseForCustomerMethod()
        {
            SelectedCustomerr = SelectedUserAppointment.Customer;
            ButtonForwardChooseCustomerMethod();
        }

        #endregion

        #region UseCase HouseConfig
        #region PROPERTIES

        #region Variablen

        private string groundPlotSize;

        public string GroundPlotSize
        {
            get { return groundPlotSize; }
            set { groundPlotSize = value; OnChange("GroundPlotSize"); }
        }

        private string groundPlotAddress;

        public string GroundPlotAddress
        {
            get { return groundPlotAddress; }
            set { groundPlotAddress = value; OnChange("GroundPlotAddress"); }
        }
        private string groundPlotZip;

        public string GroundPlotZip
        {
            get { return groundPlotZip; }
            set { groundPlotZip = value; OnChange("GroundPlotZip"); }
        }


        private int newGroundPlotId0 = 0;

        public int NewGroundPlotId0
        {
            get { return newGroundPlotId0; }
            set { newGroundPlotId0 = value; }
        }
        private int newGroundPlotId1 = 0;

        public int NewGroundPlotId1
        {
            get { return newGroundPlotId1; }
            set { newGroundPlotId1 = value; }
        }
        private int newGroundPlotId2 = 0;

        public int NewGroundPlotId2
        {
            get { return newGroundPlotId2; }
            set { newGroundPlotId2 = value; }
        }


        public int NewPlotId { get; set; }
        private string buttonForwardChooseCustomerVisibility = "Collapsed";

        public string ButtonForwardChooseCustomerVisibility
        {
            get { return buttonForwardChooseCustomerVisibility; }
            set { buttonForwardChooseCustomerVisibility = value; OnChange("ButtonForwardChooseCustomerVisibility"); }
        }
        //Höchste configId auslesen
        public Houseconfig configId { get; set; }
        //SQLite pfad variable
        public string DbPath { get; set; }
        //Button 1-11 wird sichtbar
        private string buttonIsVisible = "Collapsed";

        public string ButtonIsVisible
        {
            get { return buttonIsVisible; }
            set { buttonIsVisible = value; OnChange("ButtonIsVisible"); }
        }

        //Zusammenfassung des konfigurierten Hauses
        private HouseSummary house;

        public HouseSummary House
        {
            get { return house; }
            set { house = value; OnChange("House"); }
        }


        //Der gesammte Preis
        private double totalPrice = 0;

        public double TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; OnChange("TotalPrice"); }
        }

        //Anzahl der Stockwerke aus der DB
        private int numberOfFloorDB;

        public int NumberOfFloorDB
        {
            get { return numberOfFloorDB; }
            set { numberOfFloorDB = value; }
        }
        private string groundPlotInfoVisibility = "Collapsed";

        public string GroundPlotInfoVisibility
        {
            get { return groundPlotInfoVisibility; }
            set { groundPlotInfoVisibility = value; OnChange("GroundPlotInfoVisibility"); }
        }


        //Frame wird initialisiert - damit wir die Frame, wo alle Pages angezeigt werden haben
        Frame a = new Frame();

        #endregion

        #region Listen
        //Customer die in der Liste gespeichert sind
        private ObservableCollection<Customer> customers = new ObservableCollection<Customer>();
        public ObservableCollection<Customer> Customers
        {
            get { return customers; }
            set { customers = value; OnChange("Items"); }
        }


        //Liste für Häuse aus der DB
        private ObservableCollection<ImageInherit> imagesHouse = new ObservableCollection<ImageInherit>();
        public ObservableCollection<ImageInherit> ImagesHouse
        {
            get { return imagesHouse; }
            set { imagesHouse = value; OnChange("ImagesHouse"); }
        }
        //Liste für Grundstücke
        private ObservableCollection<ImageInherit> imagesPlot = new ObservableCollection<ImageInherit>();
        public ObservableCollection<ImageInherit> ImagesPlot
        {
            get { return imagesPlot; }
            set { imagesPlot = value; OnChange("ImagesPlot"); }
        }

        //Die Anzahl der Stockwerke
        private ObservableCollection<int> numberFloors = new ObservableCollection<int>();

        public ObservableCollection<int> NumberFloors
        {
            get { return numberFloors; }
            set { numberFloors = value; }
        }

        //Die Liste für die Grundrisse der Stockwerke
        private ObservableCollection<ImageInherit> floorsGroundPlot = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> FloorsGroundPlot
        {
            get { return floorsGroundPlot; }
            set { floorsGroundPlot = value; }
        }

        //Liste Außenwände
        private ObservableCollection<ImageInherit> listOutsideWall = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListOutsideWall
        {
            get { return listOutsideWall; }
            set { listOutsideWall = value; }
        }

        //Liste für Farben der Außenwände
        private ObservableCollection<ColorPalette> listColorOutsideWall = new ObservableCollection<ColorPalette>();

        public ObservableCollection<ColorPalette> ListColorOutsideWall
        {
            get { return listColorOutsideWall; }
            set { listColorOutsideWall = value; }
        }


        //Liste für die Innenwände
        private ObservableCollection<ImageInherit> listInsideWall = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListInsideWall
        {
            get { return listInsideWall; }
            set { listInsideWall = value; }
        }
        //Liste für die Farben der Innenwände
        private ObservableCollection<ColorPalette> listColorInsideWall = new ObservableCollection<ColorPalette>();

        public ObservableCollection<ColorPalette> ListColorInsideWall
        {
            get { return listColorInsideWall; }
            set { listColorInsideWall = value; }
        }

        //Liste für die Dachtypen
        private ObservableCollection<ImageInherit> listRoofType = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListRoofType
        {
            get { return listRoofType; }
            set { listRoofType = value; }
        }

        //Liste fpr das Dachmaterial
        private ObservableCollection<ImageInherit> listRoofMaterial = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListRoofMaterial
        {
            get { return listRoofMaterial; }
            set { listRoofMaterial = value; }
        }

        //Liste für die Fenster
        private ObservableCollection<ImageInherit> listWindows = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListWindows
        {
            get { return listWindows; }
            set { listWindows = value; }
        }

        //Liste für die Farben der Fenster
        private ObservableCollection<ColorPalette> listColorWindows = new ObservableCollection<ColorPalette>();

        public ObservableCollection<ColorPalette> ListColorWindows
        {
            get { return listColorWindows; }
            set { listColorWindows = value; OnChange("ListColorWindows"); }
        }


        //Liste für die Türen
        private ObservableCollection<ImageInherit> listDoors = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListDoors
        {
            get { return listDoors; }
            set { listDoors = value; }
        }

        //Liste für Farben der Türen
        private ObservableCollection<ColorPalette> listColorDoors = new ObservableCollection<ColorPalette>();

        public ObservableCollection<ColorPalette> ListColorDoors
        {
            get { return listColorDoors; }
            set { listColorDoors = value; OnChange("ListColorDoors"); }
        }

        //Liste für die Energiesysteme
        private ObservableCollection<EHSystem> listEnergySystem = new ObservableCollection<EHSystem>();

        public ObservableCollection<EHSystem> ListEnergySystem
        {
            get { return listEnergySystem; }
            set { listEnergySystem = value; }
        }

        //Liste für die Heizungssysteme
        private ObservableCollection<EHSystem> listHeatingSystem = new ObservableCollection<EHSystem>();

        public ObservableCollection<EHSystem> ListHeatingSystem
        {
            get { return listHeatingSystem; }
            set { listHeatingSystem = value; }
        }

        //Liste für Anzahl der Steckdosen
        private ObservableCollection<ImageInherit> numberSockets = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> NumberSockets
        {
            get { return numberSockets; }
            set { numberSockets = value; }
        }

        //Liste für die Kamine
        private ObservableCollection<ImageInherit> listChimneys = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListChimneys
        {
            get { return listChimneys; }
            set { listChimneys = value; }
        }

        //Liste für die Quadratmeter der Pools
        private ObservableCollection<ImageInherit> numberPoolSizes = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> NumberPoolSizes
        {
            get { return numberPoolSizes; }
            set { numberPoolSizes = value; }
        }

        //Liste der SwimmingPools
        private ObservableCollection<ImageInherit> listPools = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListPools
        {
            get { return listPools; }
            set { listPools = value; }
        }

        //Liste der Zäune
        private ObservableCollection<ImageInherit> listFence = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListFence
        {
            get { return listFence; }
            set { listFence = value; }
        }

        //Liste für die Farben der Zäune
        private ObservableCollection<ColorPalette> listColorFence = new ObservableCollection<ColorPalette>();

        public ObservableCollection<ColorPalette> ListColorFence
        {
            get { return listColorFence; }
            set { listColorFence = value; }
        }



        #endregion

        #region Selected Items

        // der ausgewählte Kunde in der Kundenliste
        private Customer selectedCustomerr;
        public Customer SelectedCustomerr
        {
            get { return selectedCustomerr; }
            set
            {
                selectedCustomerr = value;
                OnChange("SelectedCustomerr");
                ButtonForwardChooseCustomerVisibility = "Visible";
            }
        }

        //selected Anzahl der Stockwerk
        private int selectedItemFloor;
        public int SelectedItemFloor
        {
            get { return selectedItemFloor; }
            set
            {
                selectedItemFloor = value;
                ChooseGroundPlots();
                selectedFloor = FloorsGroundPlot[0];
            }
        }

        //selected Haus
        private ImageInherit selectedHouse;
        public ImageInherit SelectedHouse
        {
            get { return selectedHouse; }
            set
            {
                selectedHouse = value;
                OnChange("SelectedHouse");
                SetPackageProperties();
            }
        }

        //selected Grundstück
        private ImageInherit selectedPlot;
        public ImageInherit SelectedPlot
        {
            get { return selectedPlot; }
            set { selectedPlot = value; OnChange("SelectedPlot"); }
        }

        //selected Grundriss der Stockwerke
        private ImageInherit selectedFloor;
        public ImageInherit SelectedFloor
        {
            get { return selectedFloor; }
            set { selectedFloor = value; OnChange("SelectedFloor"); }
        }

        //selected Außenwand
        private ImageInherit selectedOutsideWall;
        public ImageInherit SelectedOutsideWall
        {
            get { return selectedOutsideWall; }
            set { selectedOutsideWall = value; OnChange("SelectedOutsideWall"); }
        }

        //selected Farben Außenwände
        private ColorPalette selectedColorOutsideWall;
        public ColorPalette SelectedColorOutsideWall
        {
            get { return selectedColorOutsideWall; }
            set { selectedColorOutsideWall = value; OnChange("SelectedColorOutsideWall"); }
        }

        //selected Innside
        private ImageInherit selectedInsideWall;
        public ImageInherit SelectedInsideWall
        {
            get { return selectedInsideWall; }
            set { selectedInsideWall = value; OnChange("SelectedInsideWall"); }
        }

        //selected Farbe Innenwände
        private ColorPalette selectedColorInsideWall;
        public ColorPalette SelectedColorInsideWall
        {
            get { return selectedColorInsideWall; }
            set { selectedColorInsideWall = value; OnChange("SelectedColorInsideWall"); }
        }

        //selected Dach Typ
        private ImageInherit selectedRoofType;
        public ImageInherit SelectedRoofType
        {
            get { return selectedRoofType; }
            set { selectedRoofType = value; OnChange("SelectedRoofType"); }
        }

        //selected Dach Typ
        private ImageInherit selectedRoofMaterial;
        public ImageInherit SelectedRoofMaterial
        {
            get { return selectedRoofMaterial; }
            set { selectedRoofMaterial = value; OnChange("SelectedRoofMaterial"); }
        }

        //Selected Fenster
        private ImageInherit selectedWindow;
        public ImageInherit SelectedWindow
        {
            get { return selectedWindow; }
            set { selectedWindow = value; OnChange("SelectedWindow"); }
        }

        //selected Farbe für Fenster
        private ColorPalette selectedColorWindow;
        public ColorPalette SelectedColorWindow
        {
            get { return selectedColorWindow; }
            set { selectedColorWindow = value; OnChange("SelectedColorWindow"); }
        }

        //Selected Tür
        private ImageInherit selectedDoor;
        public ImageInherit SelectedDoor
        {
            get { return selectedDoor; }
            set { selectedDoor = value; OnChange("SelectedDoor"); }
        }

        //selected Farbe für Türen
        private ColorPalette selectedColorDoor;
        public ColorPalette SelectedColorDoor
        {
            get { return selectedColorDoor; }
            set { selectedColorDoor = value; OnChange("SelectedColorDoor"); }
        }

        //selected Energiesystem
        private EHSystem selectedEnergySystem;
        public EHSystem SelectedEnergySystem
        {
            get { return selectedEnergySystem; }
            set { selectedEnergySystem = value; OnChange("SelectedEnergySystem"); }
        }

        //selected Heizungssystem
        private EHSystem selectedHeatingSystem;
        public EHSystem SelectedHeatingSystem
        {
            get { return selectedHeatingSystem; }
            set { selectedHeatingSystem = value; OnChange("SelectedHeatingSystem"); }
        }

        //selected Anzahl der Steckdosen
        private ImageInherit selectedSocket;
        public ImageInherit SelectedSocket
        {
            get { return selectedSocket; }
            set { selectedSocket = value; OnChange("SelectedSocket"); }
        }

        //selected Kamin
        private ImageInherit selectedChimney;
        public ImageInherit SelectedChimney
        {
            get { return selectedChimney; }
            set { selectedChimney = value; OnChange("SelectedChimney"); }
        }

        //selected Größen für die Pools
        private ImageInherit selectedPoolSize;
        public ImageInherit SelectedPoolSize
        {
            get { return selectedPoolSize; }
            set { selectedPoolSize = value; OnChange("SelectedPoolSize"); }
        }

        //selected Pool
        private ImageInherit selectedPool;

        public ImageInherit SelectedPool
        {
            get { return selectedPool; }
            set { selectedPool = value; OnChange("SelectedPool"); }
        }

        //selected Zaun
        private ImageInherit selectedFence;
        public ImageInherit SelectedFence
        {
            get { return selectedFence; }
            set { selectedFence = value; OnChange("SelectedFence"); }
        }

        //selected Farbe Zaun
        private ColorPalette selectedColorFence;
        public ColorPalette SelectedColorFence
        {
            get { return selectedColorFence; }
            set { selectedColorFence = value; OnChange("SelectedColorFence"); }
        }

        //SelectedGarage
        private bool checkBoxIsSelected;

        public bool CheckBoxIsSelected
        {
            get { return checkBoxIsSelected; }
            set { checkBoxIsSelected = value; OnChange("CheckBoxIsSelected"); }
        }


        #endregion

        #region Commands
        //Leitet den User zu Schritt 2
        public RelayCommand ButtonForwardChooseCustomer { get; set; }
        //Leitet den User zu Schritt 3
        public RelayCommand ButtonForwardChooseHouse { get; set; }
        //Leitet den User zu Schritt 4
        public RelayCommand ButtonForwardChoosePlot { get; set; }
        //Leitet den User zu Schritt 5 -> Wand
        public RelayCommand ButtonForwardChooseWall { get; set; }
        //Leitet den User zu Schritt 6 -> Dach
        public RelayCommand ButtonForwardChooseRoof { get; set; }
        //Leitet den User zu Schritt 7 -> Fenster und Türen
        public RelayCommand ButtonForwardChooseWindowsDoors { get; set; }
        //Leitet den User zu Schritt 8 -> Energy und Heizung
        public RelayCommand ButtonForwardChooseEnergie { get; set; }
        //Leitet den User zu Schritt 9 -> ElektroInstallaiton und Kamin
        public RelayCommand ButtonForwardChooseAddition { get; set; }
        //Leitet den User zu Schritt 10 -> Außenbereich definieren
        public RelayCommand ButtonForwardChooseOutsideArea { get; set; }
        //Leitet den User zu Schritt 11 -> Zusammenfassung
        public RelayCommand ButtonForwardSummary { get; set; }
        //Leitet den User zu Schritt 2 zurück -> HausAuswahl
        public RelayCommand ButtonEditConfiguration { get; set; }
        //Speichert das konfigurierte Haus in der DB
        public RelayCommand ButtonSaveConfiguration { get; set; }
        //Das konfigurierte Haus wird in ndie Datenbank gespeichert -> Projekt wurde erstellt
        public RelayCommand ButtonCreateProject { get; set; }
        //Upload Grundstück
        public RelayCommand ButtonUploadPlot { get; set; }
        //Grundstücksinfo Hinzufügen
        public RelayCommand ButtonAddGroundPlotInfo { get; set; }
        //Löschen des Grundstückes
        #endregion

        #region Notizen
        private string noteStep2;

        public string NoteStep2
        {
            get { return noteStep2; }
            set { noteStep2 = value; OnChange("NoteStep2"); }
        }
        private string noteStep3;

        public string NoteStep3
        {
            get { return noteStep3; }
            set { noteStep3 = value; OnChange("NoteStep3"); }
        }

        private string noteStep4;

        public string NoteStep4
        {
            get { return noteStep4; }
            set { noteStep4 = value; OnChange("NoteStep4"); }
        }

        private string noteStep5_1;

        public string NoteStep5_1
        {
            get { return noteStep5_1; }
            set { noteStep5_1 = value; OnChange("NoteStep5_1"); }
        }
        private string noteStep5_2;

        public string NoteStep5_2
        {
            get { return noteStep5_2; }
            set { noteStep5_2 = value; OnChange("NoteStep5_2"); }
        }

        private string noteStep6;

        public string NoteStep6
        {
            get { return noteStep6; }
            set { noteStep6 = value; OnChange("NoteStep6"); }
        }

        private string noteStep7_1;

        public string NoteStep7_1
        {
            get { return noteStep7_1; }
            set { noteStep7_1 = value; OnChange("NoteStep7_1"); }
        }
        private string noteStep7_2;

        public string NoteStep7_2
        {
            get { return noteStep7_2; }
            set { noteStep7_2 = value; OnChange("NoteStep7_2"); }
        }

        private string noteStep8;

        public string NoteStep8
        {
            get { return noteStep8; }
            set { noteStep8 = value; OnChange("NoteStep8"); }
        }

        private string noteStep9;

        public string NoteStep9
        {
            get { return noteStep9; }
            set { noteStep9 = value; OnChange("NoteStep9"); }
        }

        private string noteStep10_1;

        public string NoteStep10_1
        {
            get { return noteStep10_1; }
            set { noteStep10_1 = value; OnChange("NoteStep10_1"); }
        }
        private string noteStep10_2;

        public string NoteStep10_2
        {
            get { return noteStep10_2; }
            set { noteStep10_2 = value; OnChange("NoteStep10_2"); }
        }
        #endregion

        #endregion

        #region ForwardButtons

        //Hier wird weitergeleitet auf Schritt 2
        private async void ButtonForwardChooseCustomerMethod()
        {
            if (selectedCustomerr != null)
            {
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt2HausAuswahl));
                //GetStackPanels();

                ImagesHouse.Clear();

                List<ImageInherit> models = SQLGetHouses();
                foreach (var item in models)
                {
                    ImagesHouse.Add(new ImageInherit(item.SourceImage, item.Id, item.Description, item.Price, item.Zip, item.City, item.Street, item.HouseNo, item.Country));
                }
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie einen Kunden aus.");
                await dialog.ShowAsync();
            }
        }
        //Hier wird weitergeleitet auf Schritt 3
        private async void ButtonForwardChooseHouseMethod()
        {
            if (SelectedHouse != null)
            {
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt3GrundstückAuswahl));
                GetTotalPrice();

                ImagesPlot.Clear();

                //Grundstücke aus der Datenbank selecten
                List<DBModel.Attribute> models = SQLGetAttribute(3);

                //Grundstückbilder werden angezeigt
                foreach (var item in models)
                {
                    ImagesPlot.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, item.price));
                }
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Haus aus.");
                await dialog.ShowAsync();
            }
        }
        //Upload Grundstück
        private async void ButtonUploadPlotMethod()
        {
            DeleteUploadedPlotMethod();
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                try
                {
                    var rootFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DreamHouse\\" + SelectedCustomerr.Id + "Plots", CreationCollisionOption.OpenIfExists); // Create folder
                    await file.CopyAsync(rootFolder, file.Name, NameCollisionOption.ReplaceExisting);
                    string fileName = file.Name.Split('.').First();
                    string filePath = rootFolder.Path + "\\" + file.Name;
                    using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                    {
                        con.Insert(new Temp_Table()
                        {
                            description = fileName,
                            price = 0,
                            rootfolder = filePath
                        });
                        NewPlotId = (con.Table<Temp_Table>().OrderByDescending(u => u.id).FirstOrDefault()).id;
                        con.Close();
                    }

                    ImagesPlot.Add(new ImageInherit(filePath, 0, fileName, 0));
                }
                catch (Exception)
                {
                    var dialog = new MessageDialog("Bitte versuchen Sie es mit einem anderen Namen für Ihre Datei!", "Error");
                    await dialog.ShowAsync();
                }
            }
        }
        public void DeleteUploadedPlotMethod()
        {
            foreach (var item in ImagesPlot.ToList())
            {
                if (item.Id == 0)
                    ImagesPlot.Remove(item);
            }
        }
        public void ButtonAddGroundPlotInfoMethod()
        {
            GroundPlotInfoVisibility = GroundPlotInfoVisibility == "Visible" ? "Collapsed" : "Visible";
        }

        //Hier wird weitergeleitet auf Schritt 4
        private async void ButtonForwardChoosePlotMethod()
        {
            if (SelectedPlot != null)
            {
                if (SelectedPlot.Id == 0 && (GroundPlotSize == null || GroundPlotAddress == null || GroundPlotZip == null))
                {
                    var dialog = new MessageDialog("Bitte die Grundstücksinformationen eingeben.");
                    await dialog.ShowAsync();

                }
                else
                {
                    SelectedPlot.Description += " " + GroundPlotSize + " " + GroundPlotAddress + " " + GroundPlotZip;
                    GetFrame();
                    a.Navigate(typeof(Pages.HKPages.Schritt4Grundriss));
                    GetTotalPrice();

                    NumberFloors.Clear();

                    //Dropdown Stockwerke befüllen
                    NumberFloors.Add(0);
                    NumberFloors.Add(1);
                    NumberFloors.Add(2);
                    try
                    {
                        //Anzahl der Stockwerke vom ausgewähltem haus aus der DB
                        using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                        {
                            NumberOfFloorDB = (from a in con.Table<Ymdh_House_Package>()
                                               where a.house_package_id.Equals(SelectedHouse.Id)
                                               select a.housefloors).Single();
                            con.Close();
                        }
                        //SelectedItemFloor = NumberOfFloorDB;
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Grundstück aus.");
                await dialog.ShowAsync();
            }
        }

        //wenn anzahl von stockwerken angeklickt wird -> anzeigen der Grundrisse
        public void ChooseGroundPlots()
        {
            FloorsGroundPlot.Clear();

            List<Housefloor_Package> floorPackage = new List<Housefloor_Package>();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    for (int i = 0; i < NumberOfFloorDB + 1; i++)
                    {
                        floorPackage.Add((from a in con.Table<Ymdh_House_Package>()
                                          from b in con.Table<Housefloor_Package>()
                                          where a.house_package_id.Equals(SelectedHouse.Id)
                                          && b.house_package_id.Equals(a.house_package_id)
                                          && b.area.Equals(i)
                                          select b).Single());
                    }
                    con.Close();
                }
            }
            catch (Exception) { }
            if (floorPackage.Count == 0 && SelectedItemFloor == 0)
                FloorsGroundPlot.Add(new ImageInherit(HouseconfigFolder.Path + "/4Grundriss/weiss.png", 0, 0, new RelayCommand(DrawSketchGroundfloorMethod), new RelayCommand(GroundfloorMethod), SelectedItemFloor, NumberOfFloorDB));
            else if (floorPackage.Count == 0 && SelectedItemFloor == 1)
                FloorsGroundPlot.Add(new ImageInherit(HouseconfigFolder.Path + "/4Grundriss/weiss.png", 0, 1, new RelayCommand(DrawSketchFloor1Method), new RelayCommand(Floor1Method), SelectedItemFloor, NumberOfFloorDB));
            else if (floorPackage.Count == 0 && SelectedItemFloor == 2)
                FloorsGroundPlot.Add(new ImageInherit(HouseconfigFolder.Path + "/4Grundriss/weiss.png", 0, 2, new RelayCommand(DrawSketchFloor2Method), new RelayCommand(Floor2Method), SelectedItemFloor, NumberOfFloorDB));
            else
            {
                if (NumberOfFloorDB >= 0)
                    FloorsGroundPlot.Add(new ImageInherit(floorPackage[0].rootfolder, floorPackage[0].housefloor_id, floorPackage[0].area, new RelayCommand(DrawSketchGroundfloorMethod), new RelayCommand(GroundfloorMethod), SelectedItemFloor, NumberOfFloorDB));

                if (NumberOfFloorDB >= 1)
                    if (SelectedItemFloor == 1 || SelectedItemFloor == 2)
                        FloorsGroundPlot.Add(new ImageInherit(floorPackage[1].rootfolder, floorPackage[1].housefloor_id, floorPackage[1].area, new RelayCommand(DrawSketchFloor1Method), new RelayCommand(Floor1Method), SelectedItemFloor, NumberOfFloorDB));

                if (NumberOfFloorDB >= 2 && SelectedItemFloor == 2)
                    FloorsGroundPlot.Add(new ImageInherit(floorPackage[2].rootfolder, floorPackage[2].housefloor_id, floorPackage[2].area, new RelayCommand(DrawSketchFloor2Method), new RelayCommand(Floor2Method), SelectedItemFloor, NumberOfFloorDB));

                if (NumberOfFloorDB == 0 && SelectedItemFloor == 1)
                    FloorsGroundPlot.Add(new ImageInherit(HouseconfigFolder.Path + "/4Grundriss/weiss.png", 0, 1, new RelayCommand(DrawSketchFloor1Method), new RelayCommand(Floor1Method), SelectedItemFloor, NumberOfFloorDB));

                if (NumberOfFloorDB == 0 && SelectedItemFloor == 2)
                {
                    FloorsGroundPlot.Add(new ImageInherit(HouseconfigFolder.Path + "/4Grundriss/weiss.png", 0, 1, new RelayCommand(DrawSketchFloor1Method), new RelayCommand(Floor1Method), SelectedItemFloor, NumberOfFloorDB));
                    FloorsGroundPlot.Add(new ImageInherit(HouseconfigFolder.Path + "/4Grundriss/weiss.png", 0, 2, new RelayCommand(DrawSketchFloor2Method), new RelayCommand(Floor2Method), SelectedItemFloor, NumberOfFloorDB));
                }
                if (NumberOfFloorDB == 1 && SelectedItemFloor == 2)
                    FloorsGroundPlot.Add(new ImageInherit(HouseconfigFolder.Path + "/4Grundriss/weiss.png", 0, 2, new RelayCommand(DrawSketchFloor2Method), new RelayCommand(Floor2Method), SelectedItemFloor, NumberOfFloorDB));
            }
            OnChange("FloorsGroundPlot");
        }
        //Grundriss zeichnen Erdgeschoss
        private void DrawSketchGroundfloorMethod()
        {
            DrawSketch(0);
        }
        //Grundriss zeichnen Stockwerk 1
        private void DrawSketchFloor1Method()
        {
            DrawSketch(1);
        }
        //Grundriss zeichnen Stockwerk 2
        private void DrawSketchFloor2Method()
        {
            DrawSketch(2);
        }
        //Upload Grundriss Erdgeschoss
        private void GroundfloorMethod()
        {
            UploadGroundplot("Erdgeschoss", 0);
        }

        //Upload Grundriss Stockwerk1
        private void Floor1Method()
        {
            UploadGroundplot("Stockwerk 1", 1);
        }

        private void Floor2Method()
        {
            UploadGroundplot("Stockwerk 2", 2);
        }

        private async void UploadGroundplot(string name, int i)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                try
                {
                    var rootFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DreamHouse\\" + SelectedCustomerr.Id + "Groundplots", CreationCollisionOption.OpenIfExists); // Create folder
                    await file.CopyAsync(rootFolder, file.Name, NameCollisionOption.ReplaceExisting);
                    string fileName = new Guid().ToString();
                    string filePath = rootFolder.Path + "\\" + file.Name;

                    using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                    {
                        con.Insert(new Temp_Table()
                        {
                            description = fileName,
                            price = 0,
                            rootfolder = filePath
                        });

                        if (i == 0)
                        {
                            NewGroundPlotId0 = (con.Table<Temp_Table>().OrderByDescending(u => u.id).FirstOrDefault()).id;
                            FloorsGroundPlot.Add(new ImageInherit(filePath, 0, i, new RelayCommand(DrawSketchGroundfloorMethod), new RelayCommand(GroundfloorMethod), SelectedItemFloor, NumberOfFloorDB));
                        }
                        else if (i == 1)
                        {
                            NewGroundPlotId1 = (con.Table<Temp_Table>().OrderByDescending(u => u.id).FirstOrDefault()).id;
                            FloorsGroundPlot.Add(new ImageInherit(filePath, 0, i, new RelayCommand(DrawSketchFloor1Method), new RelayCommand(Floor1Method), SelectedItemFloor, NumberOfFloorDB));
                        }
                        else if (i == 2)
                        {
                            NewGroundPlotId2 = (con.Table<Temp_Table>().OrderByDescending(u => u.id).FirstOrDefault()).id;
                            FloorsGroundPlot.Add(new ImageInherit(filePath, 0, i, new RelayCommand(DrawSketchFloor2Method), new RelayCommand(Floor2Method), SelectedItemFloor, NumberOfFloorDB));
                        }
                        FloorsGroundPlot.RemoveAt(i);
                        FloorsGroundPlot.Move(FloorsGroundPlot.Count - 1, i);

                        con.Close();
                    }
                }
                catch (Exception)
                {
                    var dialog = new MessageDialog("Ein Fehler ist aufgetreten!", "Error");
                    await dialog.ShowAsync();
                }
            }
        }

        //Hier wird weitergeleitet auf Schritt 5
        private async void ButtonForwardChooseWallMethod()
        {
            if (SelectedFloor != null)
            {
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt5Wand));
                GetTotalPrice();

                ListOutsideWall.Clear();
                ListColorOutsideWall.Clear();
                ListInsideWall.Clear();
                ListColorInsideWall.Clear();

                //Außenwände aus der Datenbank selecten
                List<DBModel.Attribute> models = SQLGetAttribute(5);

                //Außenwände werden angezeigt
                foreach (var item in models)
                {
                    if (SelectedOutsideWall.Id == item.attribute_id)
                        ListOutsideWall.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, 0));
                    else
                        ListOutsideWall.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, item.price));
                }

                //Farben Außenwände aus der Datenbank selecten
                List<DBModel.Attribute> modelsC = SQLGetAttribute(902);

                //Farben Außenwände werden angezeigt
                foreach (var item in modelsC)
                {
                    string[] color = item.description.Split(',');
                    byte r = Byte.Parse(color[0]);
                    byte g = Byte.Parse(color[1]);
                    byte b = Byte.Parse(color[2]);
                    ListColorOutsideWall.Add(new ColorPalette(item.attribute_id, r, g, b));
                }

                //Innenwände aus der Datenbank selecten
                List<DBModel.Attribute> models2 = SQLGetAttribute(51);

                //Innenwände werden angezeigt
                foreach (var item in models2)
                {
                    if (SelectedInsideWall.Id == item.attribute_id)
                        ListInsideWall.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, 0));
                    else
                        ListInsideWall.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, item.price));
                }

                //Farben Innenwände aus der Datenbank selecten
                List<DBModel.Attribute> models2C = SQLGetAttribute(901);

                //Farben Innenwände werden angezeigt
                foreach (var item in models2C)
                {
                    string[] color = item.description.Split(',');
                    byte r = Byte.Parse(color[0]);
                    byte g = Byte.Parse(color[1]);
                    byte b = Byte.Parse(color[2]);
                    ListColorInsideWall.Add(new ColorPalette(item.attribute_id, r, g, b));
                }
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Grundriss aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier wird weitergeleitet auf Schritt 6 
        private async void ButtonForwardChooseRoofMethod()
        {
            if (SelectedInsideWall != null && selectedOutsideWall != null)
            {
                GetTotalPrice();
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt6Dach));

                ListRoofType.Clear();
                ListRoofMaterial.Clear();

                //Dachtypen aus der Datenbank selecten
                List<DBModel.Attribute> models = SQLGetAttribute(6);

                //Dachtypen werden angezeigt
                foreach (var item in models)
                {
                    if (SelectedRoofType.Id == item.attribute_id)
                        ListRoofType.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, 0));
                    else
                        ListRoofType.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, item.price));
                }

                //Dachmaterial aus der Datenbank selecten
                List<DBModel.Attribute> models2 = SQLGetAttribute(61);

                //Dachmaterial werden angezeigt
                foreach (var item in models2)
                {
                    if (SelectedRoofMaterial.Id == item.attribute_id)
                        ListRoofMaterial.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, 0));
                    else
                        ListRoofMaterial.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, item.price));
                }
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie den Typen der Außen- und Innenwand aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier wird weitergeleitet auf Schritt 7
        private async void ButtonForwardChooseWindowsDoorsMethod()
        {
            if (selectedRoofType != null && SelectedRoofMaterial != null)
            {
                GetTotalPrice();
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt7FensterTueren));

                ListWindows.Clear();
                ListColorWindows.Clear();
                ListDoors.Clear();
                ListColorDoors.Clear();

                //Fenster aus der Datenbank selecten
                List<DBModel.Attribute> models = SQLGetAttribute(7);

                //Fenster werden angezeigt
                foreach (var item in models)
                {
                    if (SelectedWindow.Id == item.attribute_id)
                        ListWindows.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, 0));
                    else
                        ListWindows.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, item.price));
                }

                //Farben Fenster aus der Datenbank selecten
                List<DBModel.Attribute> modelsC = SQLGetAttribute(903);

                //Farben Fenster werden angezeigt
                foreach (var item in modelsC)
                {
                    string[] color = item.description.Split(',');
                    byte r = Byte.Parse(color[0]);
                    byte g = Byte.Parse(color[1]);
                    byte b = Byte.Parse(color[2]);
                    ListColorWindows.Add(new ColorPalette(item.attribute_id, r, g, b));
                }

                //Türen aus der Datenbank selecten
                List<DBModel.Attribute> models2 = SQLGetAttribute(71);

                //Türen werden angezeigt
                foreach (var item in models2)
                {
                    if (SelectedDoor.Id == item.attribute_id)
                        ListDoors.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, 0));
                    else
                        ListDoors.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, item.price));
                }

                //Farben Türen aus der Datenbank selecten
                List<DBModel.Attribute> models2C = SQLGetAttribute(904);

                //Farben Türen werden angezeigt
                foreach (var item in models2C)
                {
                    string[] color = item.description.Split(',');
                    byte r = Byte.Parse(color[0]);
                    byte g = Byte.Parse(color[1]);
                    byte b = Byte.Parse(color[2]);
                    ListColorDoors.Add(new ColorPalette(item.attribute_id, r, g, b));
                }
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Dachtyp und ein Dachmaterial aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier wird zum Schritt 8 weitergeleitet
        private async void ButtonForwardChooseEnergieMethod()
        {

            if (SelectedWindow != null && SelectedDoor != null)
            {
                ListEnergySystem.Clear();
                ListHeatingSystem.Clear();

                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt8EnergyHeizung));

                //Energiesystem aus der Datenbank selecten
                List<DBModel.Attribute> models = SQLGetAttribute(8);

                //Energiesystem werden angezeigt
                foreach (var item in models)
                {
                    if (SelectedEnergySystem.Id == item.attribute_id)
                        ListEnergySystem.Add(new EHSystem(item.attribute_id, item.description, 0));
                    else
                        ListEnergySystem.Add(new EHSystem(item.attribute_id, item.description, item.price));
                }

                //Heizungsystem aus der Datenbank selecten
                List<DBModel.Attribute> models2 = SQLGetAttribute(81);

                //Heizungsystem werden angezeigt
                foreach (var item in models2)
                {
                    if (SelectedHeatingSystem.Id == item.attribute_id)
                        ListHeatingSystem.Add(new EHSystem(item.attribute_id, item.description, 0));
                    else
                        ListHeatingSystem.Add(new EHSystem(item.attribute_id, item.description, item.price));
                }

                GetTotalPrice();
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie Fenster und Türen aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier wird zu Schritt 9 weiterleitet
        private async void ButtonForwardChooseAdditionMethod()
        {
            if (SelectedEnergySystem != null && selectedHeatingSystem != null)
            {
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt9Zusatz));
                GetTotalPrice();

                NumberSockets.Clear();
                ListChimneys.Clear();

                //Steckdosen aus der Datenbank selecten
                List<DBModel.Attribute> modelsS = SQLGetAttribute(12);

                //Steckdosen werden angezeigt
                foreach (var item in modelsS)
                {
                    NumberSockets.Add(new ImageInherit(item.attribute_id, item.description, item.price));
                }

                //Kamin aus der Datenbank selecten
                List<DBModel.Attribute> models = SQLGetAttribute(9);

                //Kamin werden angezeigt
                foreach (var item in models)
                {
                    if (SelectedChimney.Id == item.attribute_id)
                        ListChimneys.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, 0));
                    else
                        ListChimneys.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, item.price));
                }
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Energie- und ein Heizungsystem aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier wird zu Schritt 10 weitergeleitet
        private async void ButtonForwardChooseOutsideAreaMethod()
        {
            if (SelectedChimney != null)
            {
                if (SelectedSocket == null)
                {
                    SelectedSocket = new ImageInherit(64, "Keine Angabe", 0);
                }
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt10Aussenbereiche));

                NumberPoolSizes.Clear();
                ListPools.Clear();
                ListFence.Clear();
                ListColorFence.Clear();

                //Poolgrößen aus der Datenbank selecten
                List<DBModel.Attribute> modelsP = SQLGetAttribute(11);

                //Poolgrößen werden angezeigt
                foreach (var item in modelsP)
                {
                    NumberPoolSizes.Add(new ImageInherit(item.attribute_id, item.description, item.price));
                }

                //Pools aus der Datenbank selecten
                List<DBModel.Attribute> models = SQLGetAttribute(10);

                //Pools werden angezeigt
                foreach (var item in models)
                {
                    if (SelectedPool.Id == item.attribute_id)
                        ListPools.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, 0));
                    else
                        ListPools.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, item.price));
                }

                //Zaun aus der Datenbank selecten
                List<DBModel.Attribute> models2 = SQLGetAttribute(101);

                //Zaun werden angezeigt
                foreach (var item in models2)
                {
                    if (SelectedFence.Id == item.attribute_id)
                        ListFence.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, 0));
                    else
                        ListFence.Add(new ImageInherit(item.rootfolder, item.attribute_id, item.description, item.price));
                }

                //Farben Zaun aus der Datenbank selecten
                List<DBModel.Attribute> modelsC = SQLGetAttribute(905);

                //Farben Zaun werden angezeigt
                foreach (var item in modelsC)
                {
                    string[] color = item.description.Split(',');
                    byte r = Byte.Parse(color[0]);
                    byte g = Byte.Parse(color[1]);
                    byte b = Byte.Parse(color[2]);
                    ListColorFence.Add(new ColorPalette(item.attribute_id, r, g, b));
                }

                GetTotalPrice();
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Kamin aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier wird zu Schritt 11 weitergeleitet
        private async void ButtonForwardSummaryMethod()
        {
            if (SelectedPool != null && SelectedFence != null)
            {
                if (SelectedPool.Description == "Kein Pool" && (SelectedPoolSize == null || SelectedPoolSize.Description != "0"))
                {
                    SelectedPoolSize = new ImageInherit(56, "0", 0);
                }
                if (SelectedPool.Description != "Kein Pool" && (SelectedPoolSize == null || SelectedPoolSize.Description == "0"))
                {
                    var dialog = new MessageDialog("Bitte wählen Sie eine Poolgröße (>0 m²) aus.");
                    await dialog.ShowAsync();
                }
                else
                {
                    GetFrame();
                    a.Navigate(typeof(Pages.HKPages.Schrtitt11Zusammenfassung));
                    EHSystem i = new EHSystem();
                    if (CheckBoxIsSelected)
                    {
                        i.Id = 89; i.Name = "Ja"; i.Price = 10000;
                    }
                    else
                    {
                        i.Id = 90; i.Name = "Nein"; i.Price = 0;
                    }

                    House = new HouseSummary()
                    {
                        Customer = selectedCustomerr,
                        Package = SelectedHouse,
                        Plot = SelectedPlot,
                        numberOfFloors = SelectedItemFloor,
                        GroundPlots = FloorsGroundPlot.ToList(),
                        OutsideWall = selectedOutsideWall,
                        OutsideWallColor = SelectedColorOutsideWall,
                        InsideWall = SelectedInsideWall,
                        InsideWallColor = selectedColorInsideWall,
                        RoofType = SelectedRoofType,
                        RoofMaterial = SelectedRoofMaterial,
                        Window = SelectedWindow,
                        WindowColor = SelectedColorWindow,
                        Door = SelectedDoor,
                        DoorColor = selectedColorDoor,
                        EnergySystem = SelectedEnergySystem,
                        HeatingSystem = selectedHeatingSystem,
                        NumberOfSocket = SelectedSocket.Description,
                        Chimney = SelectedChimney,
                        Pool = selectedPool,
                        Poolsize = SelectedPoolSize.Description + " m²",
                        Fence = selectedFence,
                        FenceColor = selectedColorFence,
                        Garage = i
                    };
                    GetTotalPrice();
                    ButtonIsVisible = "Visible";
                }
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Pool und ein Zaun aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier wird das Haus in die Datenbank gespeichert -> Projekt erstellen
        private async void ButtonCreateProjectMethod()
        {
           
            SQLCreateProject();
            
            var dialog = new MessageDialog("Haus wurde als Projekt erstellt");
            await dialog.ShowAsync();
            GetFrame();
            a.Navigate(typeof(Pages.ProjektePage));
            ListProjects.Clear();
            ProjectsMethod();
        }

        //Hier wird weitergeleitet auf Schritt 2 -> Konfiguration bearbeiten
        private void ButtonEditConfigurationMehtod()
        {
            GetFrame();
            a.Navigate(typeof(Pages.HKPages.Schritt2HausAuswahl));
            //GetStackPanels();
        }
        //Hier wird die Hauskonfiguration in der DB gespeichert
        private async void ButtonSaveConfigurationMethod()
        {
            SQLCreateHouseconfig();

            var dialog = new MessageDialog("Haus wurde als konfiguriertes Haus erstellt.");
            await dialog.ShowAsync();
            GetFrame();
            a.Navigate(typeof(Pages.KundenPage));
            ListCustomer.Clear();
            SQLGetCustomers();


        }
        #endregion

        #region Methods
        //Hier wird die Frame die in MainPage angezeigt wird geladen
        private void GetFrame()
        {
            a = MainPage.FrameObject.GetObject();
        }

        //Hier werden alle StackPanels inilitalisiert
        /*private void GetStackPanels()
        {
            stackPanelS2 = Pages.HKPages.Schritt2HausAuswahl.StackObjectSchritt2.GetObject();
        }*/


        private void GetTotalPrice()
        {
            TotalPrice = 0;
            TotalPrice += SelectedHouse != null ? SelectedHouse.Price : 0;
            TotalPrice += SelectedPlot != null ? SelectedPlot.Price : 0;
            //TODO: Grudnriss fehlt
            TotalPrice += SelectedOutsideWall != null ? SelectedOutsideWall.Price : 0;
            TotalPrice += SelectedInsideWall != null ? SelectedInsideWall.Price : 0;
            TotalPrice += SelectedRoofType != null ? SelectedRoofType.Price : 0;
            TotalPrice += SelectedRoofMaterial != null ? SelectedRoofMaterial.Price : 0;
            TotalPrice += SelectedWindow != null ? SelectedWindow.Price : 0;
            TotalPrice += SelectedDoor != null ? SelectedWindow.Price : 0;
            TotalPrice += selectedEnergySystem != null ? selectedEnergySystem.Price : 0;
            TotalPrice += SelectedHeatingSystem != null ? SelectedHeatingSystem.Price : 0;
            TotalPrice += SelectedChimney != null ? SelectedChimney.Price : 0;
            TotalPrice += SelectedPool != null ? SelectedPool.Price : 0;
            TotalPrice += SelectedFence != null ? SelectedFence.Price : 0;
            TotalPrice += CheckBoxIsSelected ? 10000 : 0;

        }

        private void SetPackageProperties()
        {
            try
            {
                //Setzen der SelectedItems für das ausgewählte Haus
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    DBModel.Attribute i1 = SQLGetRightAttributeForHousePackage(5, con);
                    SelectedOutsideWall = new ImageInherit(i1.rootfolder, i1.attribute_id, i1.description, 0);
                    DBModel.Attribute i2 = SQLGetRightAttributeForHousePackage(51, con);
                    SelectedInsideWall = new ImageInherit(i2.rootfolder, i2.attribute_id, i2.description, 0);
                    DBModel.Attribute i3 = SQLGetRightAttributeForHousePackage(6, con);
                    SelectedRoofType = new ImageInherit(i3.rootfolder, i3.attribute_id, i3.description, 0);
                    DBModel.Attribute i4 = SQLGetRightAttributeForHousePackage(61, con);
                    SelectedRoofMaterial = new ImageInherit(i4.rootfolder, i4.attribute_id, i4.description, 0);
                    DBModel.Attribute i5 = SQLGetRightAttributeForHousePackage(7, con);
                    SelectedWindow = new ImageInherit(i5.rootfolder, i5.attribute_id, i5.description, 0);
                    DBModel.Attribute i6 = SQLGetRightAttributeForHousePackage(71, con);
                    SelectedDoor = new ImageInherit(i6.rootfolder, i6.attribute_id, i6.description, 0);
                    DBModel.Attribute i7 = SQLGetRightAttributeForHousePackage(8, con);
                    SelectedEnergySystem = new EHSystem(i7.attribute_id, i7.description, 0);
                    DBModel.Attribute i8 = SQLGetRightAttributeForHousePackage(81, con);
                    SelectedHeatingSystem = new EHSystem(i8.attribute_id, i8.description, 0);
                    DBModel.Attribute i9 = SQLGetRightAttributeForHousePackage(9, con);
                    SelectedChimney = new ImageInherit(i9.rootfolder, i9.attribute_id, i9.description, 0);
                    DBModel.Attribute i10 = SQLGetRightAttributeForHousePackage(10, con);
                    SelectedPool = new ImageInherit(i10.rootfolder, i10.attribute_id, i10.description, 0);
                    DBModel.Attribute i11 = SQLGetRightAttributeForHousePackage(101, con);
                    SelectedFence = new ImageInherit(i11.rootfolder, i11.attribute_id, i11.description, 0);

                    if (i10.description != "Kein Pool")
                    {
                        DBModel.Attribute i12 = SQLGetRightAttributeForHousePackage(11, con);
                        SelectedPoolSize = new ImageInherit(i12.attribute_id, i12.description, 0);
                    }
                    con.Close();
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region SQL Befehle

        public void SQLCreateTable()
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    con.CreateTable<DBModel.Attribute>();
                    con.CreateTable<Attribute_Group>();
                    con.CreateTable<Houseconfig>();
                    con.CreateTable<Houseconfig_Has_Attribute>();
                    con.CreateTable<Housefloor>();
                    con.CreateTable<Housefloor_Package>();
                    con.CreateTable<Mdh_User_Usergroup_Map>();
                    con.CreateTable<Mdh_Usergroups>();
                    con.CreateTable<Mdh_Users>();
                    con.CreateTable<Package_Not_Attribute>();
                    con.CreateTable<Project>();
                    con.CreateTable<Ymdh_Address>();
                    con.CreateTable<Ymdh_Appointment>();
                    con.CreateTable<Ymdh_Appointment_Status>();
                    con.CreateTable<Ymdh_House_Package>();
                    con.CreateTable<Ymdh_House_Package_Status>();
                    con.CreateTable<Ymdh_Message>();
                    con.CreateTable<Ymdh_Person>();
                    con.CreateTable<Ymdh_Producer>();
                    con.CreateTable<Temp_Table>();
                    con.CreateTable<Mdh_Change>();
                    con.Close();
                }
            }
            catch (Exception)
            {
                
            }
        }

        public void SQLGetCustomers()
        {
            Customers.Clear();
            ListCustomer.Clear();
            List<Mdh_Users> model;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    model = (from a in con.Table<Mdh_Users>()
                             from b in con.Table<Mdh_User_Usergroup_Map>()
                             from c in con.Table<Mdh_Usergroups>()
                             where a.id.Equals(b.user_id)
                             && b.group_id.Equals(c.id)
                             && c.title.Equals("Customer")
                             select a).ToList();
                    for (int i = 0; i < model.Count; i++)
                    {
                        Customers.Add(new Customer(model[i].id, model[i].name, SQLCustomerCountProject(model[i].id, con), SQLCustomerCountHouseconfig(model[i].id, con)));
                        ListCustomer.Add(new Customer(model[i].id, model[i].name, SQLCustomerCountProject(model[i].id, con), SQLCustomerCountHouseconfig(model[i].id, con)));
                    }
                    con.Close();
                }
            }
            catch (Exception)
            {
                
            }
        }

        //Attribut Gruppen werden erstellt        
        public void SQLInsertAttributeGroup()
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    //Conn.Execute("PRAGMA foreign_keys = '1';");
                    //Attribut Grundstück
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 3,
                        description = "Grundstück",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Grundriss
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 4,
                        description = "Grundriss",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Außenwand
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 5,
                        description = "Außenwand",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Dachtyp
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 6,
                        description = "Dachtyp",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Fenster
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 7,
                        description = "Fenster",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Energiesysteme
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 8,
                        description = "Energiesysteme",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Zusatz (Kamin)
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 9,
                        description = "Kamin",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Außenbereiche (Pool)
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 10,
                        description = "Pool",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Innenwand
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 51,
                        description = "Innenwand",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Dachmaterial
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 61,
                        description = "Dachmaterial",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Türen
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 71,
                        description = "Türen",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Heizungsysteme
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 81,
                        description = "Heizungsysteme",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Außenbereiche (Zaun)
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 101,
                        description = "Zaun",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Poolgröße
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 11,
                        description = "Poolgröße",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Steckdosen
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 12,
                        description = "Steckdosen",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Garage
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 13,
                        description = "Garage",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Innenwandfarben
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 901,
                        description = "Innenwandfarben",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Außenwandfarben
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 902,
                        description = "Außenwandfarben",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Fensterfarben
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 903,
                        description = "Fensterfarben",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Türfarben
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 904,
                        description = "Türfarben",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //Attribut Zaunfarben
                    con.Insert(new Attribute_Group
                    {
                        attribute_group_id = 905,
                        description = "Zaunfarben",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    con.Close();
                }
            }
            catch (Exception)
            {
                
            }
        }
        private void SQLResetTempTable()
        {
            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
            {
                var query = (from x in con.Table<Temp_Table>()
                             select x).ToList();
                foreach (var item in query)
                {
                    con.Delete<Temp_Table>(item.id);
                }
                con.Close();
            }
        }
        //TODO insert housefloor sketch
        //Houseconfig wird erstellt
        public void SQLCreateHouseconfig()
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    //houseconfig
                    con.Insert(new Houseconfig
                    {
                        price = int.Parse(TotalPrice.ToString()),
                        status = "1",
                        price_floor = int.Parse(SelectedFloor.Price.ToString()),
                        modifieddate = ConvertDateTime(DateTime.Now),
                        house_package_id = SelectedHouse.Id,
                        consultant_user_id = SelectedConsultant.Id,
                        customer_user_id = SelectedCustomerr.Id
                    });
                    //Get houseconfig Id von gerade erstelltem Houseconfig
                    configId = (con.Table<Houseconfig>().OrderByDescending(u => u.houseconfig_id).FirstOrDefault());
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig",
                        id = configId.houseconfig_id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    //GroundPlot
                    if (NewGroundPlotId0 != 0)
                    {
                        con.Insert(new Housefloor
                        {
                            price = int.Parse(SelectedFloor.Price.ToString()),
                            sketch = "NULL",
                            modifieddate = ConvertDateTime(DateTime.Now),
                            houseconfig_id = configId.houseconfig_id,
                            area = 0,
                            rootfolder = FloorsGroundPlot[0].SourceImage
                        });
                        con.Delete<Temp_Table>(NewGroundPlotId0);
                    }
                    else
                    {
                        con.Insert(new Housefloor
                        {
                            price = int.Parse(SelectedFloor.Price.ToString()),
                            sketch = "",//(from a in con.Table<Housefloor_Package>() where a.rootfolder.Equals(FloorsGroundPlot[0].SourceImage) select a.sketch).Single(),
                            modifieddate = ConvertDateTime(DateTime.Now),
                            houseconfig_id = configId.houseconfig_id,
                            area = 0,
                            rootfolder = FloorsGroundPlot[0].SourceImage
                        });
                    }
                    Housefloor FloorId0 = (con.Table<Housefloor>().OrderByDescending(u => u.housefloor_id).FirstOrDefault());
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "housefloor",
                        id = FloorId0.housefloor_id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    if (NewGroundPlotId1 != 0 && SelectedItemFloor > 0)
                    {
                        con.Insert(new Housefloor
                        {
                            price = int.Parse(SelectedFloor.Price.ToString()),
                            sketch = "NULL",
                            modifieddate = ConvertDateTime(DateTime.Now),
                            houseconfig_id = configId.houseconfig_id,
                            area = 1,
                            rootfolder = FloorsGroundPlot[1].SourceImage
                        });
                        Housefloor FloorId1 = (con.Table<Housefloor>().OrderByDescending(u => u.housefloor_id).FirstOrDefault());
                        //sync change tabelle
                        con.Insert(new Mdh_Change
                        {
                            ctable = "housefloor",
                            id = FloorId1.housefloor_id,
                            change = "insert",
                            dt = ConvertDateTime(DateTime.Now),
                            synced = 0
                        });
                        con.Delete<Temp_Table>(NewGroundPlotId1);
                    }
                    else if (NewGroundPlotId1 == 0 && SelectedItemFloor > 0)
                    {
                        con.Insert(new Housefloor
                        {
                            price = int.Parse(SelectedFloor.Price.ToString()),
                            sketch = "",//(from a in con.Table<Housefloor_Package>() where a.rootfolder.Equals(FloorsGroundPlot[1].SourceImage) select a.sketch).Single(),
                            modifieddate = ConvertDateTime(DateTime.Now),
                            houseconfig_id = configId.houseconfig_id,
                            area = 1,
                            rootfolder = FloorsGroundPlot[1].SourceImage
                        });
                        Housefloor FloorId1 = (con.Table<Housefloor>().OrderByDescending(u => u.housefloor_id).FirstOrDefault());
                        //sync change tabelle
                        con.Insert(new Mdh_Change
                        {
                            ctable = "housefloor",
                            id = FloorId1.housefloor_id,
                            change = "insert",
                            dt = ConvertDateTime(DateTime.Now),
                            synced = 0
                        });
                    }
                    if (NewGroundPlotId2 != 0 && SelectedItemFloor > 1)
                    {
                        con.Insert(new Housefloor
                        {
                            price = int.Parse(SelectedFloor.Price.ToString()),
                            sketch = "NULL",
                            modifieddate = ConvertDateTime(DateTime.Now),
                            houseconfig_id = configId.houseconfig_id,
                            area = 2,
                            rootfolder = FloorsGroundPlot[2].SourceImage
                        });
                        Housefloor FloorId2 = (con.Table<Housefloor>().OrderByDescending(u => u.housefloor_id).FirstOrDefault());
                        //sync change tabelle
                        con.Insert(new Mdh_Change
                        {
                            ctable = "housefloor",
                            id = FloorId2.housefloor_id,
                            change = "insert",
                            dt = ConvertDateTime(DateTime.Now),
                            synced = 0
                        });
                        con.Delete<Temp_Table>(NewGroundPlotId2);
                    }
                    else if (NewGroundPlotId2 == 0 && SelectedItemFloor > 1)
                    {
                        con.Insert(new Housefloor
                        {
                            price = int.Parse(SelectedFloor.Price.ToString()),
                            sketch = "",//(from a in con.Table<Housefloor_Package>() where a.rootfolder.Equals(FloorsGroundPlot[2].SourceImage) select a.sketch).Single(),
                            modifieddate = ConvertDateTime(DateTime.Now),
                            houseconfig_id = configId.houseconfig_id,
                            area = 2,
                            rootfolder = FloorsGroundPlot[2].SourceImage
                        });
                        Housefloor FloorId2 = (con.Table<Housefloor>().OrderByDescending(u => u.housefloor_id).FirstOrDefault());
                        //sync change tabelle
                        con.Insert(new Mdh_Change
                        {
                            ctable = "housefloor",
                            id = FloorId2.housefloor_id,
                            change = "insert",
                            dt = ConvertDateTime(DateTime.Now),
                            synced = 0
                        });
                    }
                    //Plot
                    if (SelectedPlot.Id == 0)
                    {
                        con.Insert(new DBModel.Attribute()
                        {
                            description = SelectedPlot.Description,
                            price = 0,
                            image = "NULL",
                            deleted = 1,
                            modifieddate = ConvertDateTime(DateTime.Now),
                            attribute_group_id = 3,
                            rootfolder = SelectedPlot.SourceImage
                        });
                        con.Delete<Temp_Table>(NewPlotId);
                        int AttId = (con.Table<DBModel.Attribute>().OrderByDescending(u => u.attribute_id).FirstOrDefault()).attribute_id;
                        con.Insert(new Houseconfig_Has_Attribute
                        {
                            houseconfig_id = configId.houseconfig_id,
                            attribute_id = AttId,
                            amount = 1,
                            special = NoteStep3,
                            modifieddate = ConvertDateTime(DateTime.Now)
                        });
                        //sync change tabelle
                        con.Insert(new Mdh_Change
                        {
                            ctable = "attribute",
                            id = AttId,
                            change = "insert",
                            dt = ConvertDateTime(DateTime.Now)
                        });
                        //sync change tabelle
                        con.Insert(new Mdh_Change
                        {
                            ctable = "houseconfig_has_attribute",
                            id = configId.houseconfig_id,
                            id2 = AttId,
                            change = "insert",
                            dt = ConvertDateTime(DateTime.Now),
                            synced = 0
                        });
                    }
                    else
                    {
                        con.Insert(new Houseconfig_Has_Attribute
                        {
                            houseconfig_id = configId.houseconfig_id,
                            attribute_id = SelectedPlot.Id,
                            amount = 1,
                            special = NoteStep3,
                            modifieddate = ConvertDateTime(DateTime.Now)
                        });
                        //sync change tabelle
                        con.Insert(new Mdh_Change
                        {
                            ctable = "houseconfig_has_attribute",
                            id = configId.houseconfig_id,
                            id2 = SelectedPlot.Id,
                            change = "insert",
                            dt = ConvertDateTime(DateTime.Now),
                            synced = 0
                        });
                    }
                    
                    //InsideWall
                    con.Insert(new Houseconfig_Has_Attribute
                    {
                        houseconfig_id = configId.houseconfig_id,
                        attribute_id = SelectedInsideWall.Id,
                        amount = 1,
                        special = NoteStep5_2,
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig_has_attribute",
                        id = configId.houseconfig_id,
                        id2 = SelectedInsideWall.Id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    //OutsideWall
                    con.Insert(new Houseconfig_Has_Attribute
                    {
                        houseconfig_id = configId.houseconfig_id,
                        attribute_id = SelectedOutsideWall.Id,
                        amount = 1,
                        special = NoteStep5_1,
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig_has_attribute",
                        id = configId.houseconfig_id,
                        id2 = SelectedOutsideWall.Id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    //RoofType
                    con.Insert(new Houseconfig_Has_Attribute
                    {
                        houseconfig_id = configId.houseconfig_id,
                        attribute_id = SelectedRoofType.Id,
                        amount = 1,
                        special = NoteStep6,
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig_has_attribute",
                        id = configId.houseconfig_id,
                        id2 = SelectedRoofType.Id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    //RoofMaterial
                    con.Insert(new Houseconfig_Has_Attribute
                    {
                        houseconfig_id = configId.houseconfig_id,
                        attribute_id = SelectedRoofMaterial.Id,
                        amount = 1,
                        special = "",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig_has_attribute",
                        id = configId.houseconfig_id,
                        id2 = SelectedRoofMaterial.Id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    //Door
                    con.Insert(new Houseconfig_Has_Attribute
                    {
                        houseconfig_id = configId.houseconfig_id,
                        attribute_id = SelectedDoor.Id,
                        amount = 1,
                        special = NoteStep7_2,
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig_has_attribute",
                        id = configId.houseconfig_id,
                        id2 = SelectedDoor.Id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    //Window
                    con.Insert(new Houseconfig_Has_Attribute
                    {
                        houseconfig_id = configId.houseconfig_id,
                        attribute_id = SelectedWindow.Id,
                        amount = 1,
                        special = NoteStep7_1,
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig_has_attribute",
                        id = configId.houseconfig_id,
                        id2 = SelectedWindow.Id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    //EnergySystem
                    con.Insert(new Houseconfig_Has_Attribute
                    {
                        houseconfig_id = configId.houseconfig_id,
                        attribute_id = SelectedEnergySystem.Id,
                        amount = 1,
                        special = NoteStep8,
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig_has_attribute",
                        id = configId.houseconfig_id,
                        id2 = SelectedEnergySystem.Id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    //HeatingSystem
                    con.Insert(new Houseconfig_Has_Attribute
                    {
                        houseconfig_id = configId.houseconfig_id,
                        attribute_id = SelectedHeatingSystem.Id,
                        amount = 1,
                        special = "",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig_has_attribute",
                        id = configId.houseconfig_id,
                        id2 = SelectedHeatingSystem.Id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    //Pool
                    con.Insert(new Houseconfig_Has_Attribute
                    {
                        houseconfig_id = configId.houseconfig_id,
                        attribute_id = SelectedPool.Id,
                        amount = 1,
                        special = NoteStep10_1,
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig_has_attribute",
                        id = configId.houseconfig_id,
                        id2 = SelectedPool.Id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    //Fence
                    con.Insert(new Houseconfig_Has_Attribute
                    {
                        houseconfig_id = configId.houseconfig_id,
                        attribute_id = SelectedFence.Id,
                        amount = 1,
                        special = NoteStep10_2,
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig_has_attribute",
                        id = configId.houseconfig_id,
                        id2 = SelectedFence.Id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    //Chimney
                    con.Insert(new Houseconfig_Has_Attribute
                    {
                        houseconfig_id = configId.houseconfig_id,
                        attribute_id = SelectedChimney.Id,
                        amount = 1,
                        special = NoteStep9,
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig_has_attribute",
                        id = configId.houseconfig_id,
                        id2 = SelectedChimney.Id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    //PoolSize
                    con.Insert(new Houseconfig_Has_Attribute
                    {
                        houseconfig_id = configId.houseconfig_id,
                        attribute_id = SelectedPoolSize.Id,
                        amount = 1,
                        special = "",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig_has_attribute",
                        id = configId.houseconfig_id,
                        id2 = SelectedPoolSize.Id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    //Socket
                    con.Insert(new Houseconfig_Has_Attribute
                    {
                        houseconfig_id = configId.houseconfig_id,
                        attribute_id = SelectedSocket.Id,
                        amount = 1,
                        special = "",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig_has_attribute",
                        id = configId.houseconfig_id,
                        id2 = SelectedSocket.Id,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    //Garage
                    int i;
                    if (CheckBoxIsSelected)
                        i = 89;
                    else
                        i = 90;
                    con.Insert(new Houseconfig_Has_Attribute
                    {
                        houseconfig_id = configId.houseconfig_id,
                        attribute_id = i,
                        amount = 1,
                        special = "",
                        modifieddate = ConvertDateTime(DateTime.Now)
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig_has_attribute",
                        id = configId.houseconfig_id,
                        id2 = i,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    if (selectedColorInsideWall != null)
                    {
                        //Farbe Innenwand
                        con.Insert(new Houseconfig_Has_Attribute
                        {
                            houseconfig_id = configId.houseconfig_id,
                            attribute_id = SelectedColorInsideWall.Id,
                            amount = 1,
                            special = "",
                            modifieddate = ConvertDateTime(DateTime.Now)
                        });
                        //sync change tabelle
                        con.Insert(new Mdh_Change
                        {
                            ctable = "houseconfig_has_attribute",
                            id = configId.houseconfig_id,
                            id2 = SelectedColorInsideWall.Id,
                            change = "insert",
                            dt = ConvertDateTime(DateTime.Now),
                            synced = 0
                        });
                    }
                    if (SelectedColorOutsideWall != null)
                    {
                        //Farbe Außenwand
                        con.Insert(new Houseconfig_Has_Attribute
                        {
                            houseconfig_id = configId.houseconfig_id,
                            attribute_id = SelectedColorOutsideWall.Id,
                            amount = 1,
                            special = "",
                            modifieddate = ConvertDateTime(DateTime.Now)
                        });
                        //sync change tabelle
                        con.Insert(new Mdh_Change
                        {
                            ctable = "houseconfig_has_attribute",
                            id = configId.houseconfig_id,
                            id2 = SelectedColorOutsideWall.Id,
                            change = "insert",
                            dt = ConvertDateTime(DateTime.Now),
                            synced = 0
                        });
                    }
                    if (SelectedColorWindow != null)
                    {
                        //Farbe Fenster
                        con.Insert(new Houseconfig_Has_Attribute
                        {
                            houseconfig_id = configId.houseconfig_id,
                            attribute_id = SelectedColorWindow.Id,
                            amount = 1,
                            special = "",
                            modifieddate = ConvertDateTime(DateTime.Now)
                        });
                        //sync change tabelle
                        con.Insert(new Mdh_Change
                        {
                            ctable = "houseconfig_has_attribute",
                            id = configId.houseconfig_id,
                            id2 = SelectedColorWindow.Id,
                            change = "insert",
                            dt = ConvertDateTime(DateTime.Now),
                            synced = 0
                        });
                    }
                    if (SelectedColorDoor != null)
                    {
                        //Farbe Türen
                        con.Insert(new Houseconfig_Has_Attribute
                        {
                            houseconfig_id = configId.houseconfig_id,
                            attribute_id = SelectedColorDoor.Id,
                            amount = 1,
                            special = "",
                            modifieddate = ConvertDateTime(DateTime.Now)
                        });
                        //sync change tabelle
                        con.Insert(new Mdh_Change
                        {
                            ctable = "houseconfig_has_attribute",
                            id = configId.houseconfig_id,
                            id2 = SelectedColorDoor.Id,
                            change = "insert",
                            dt = ConvertDateTime(DateTime.Now),
                            synced = 0
                        });
                    }
                    if (SelectedColorFence != null)
                    {
                        //Farbe Zaun
                        con.Insert(new Houseconfig_Has_Attribute
                        {
                            houseconfig_id = configId.houseconfig_id,
                            attribute_id = SelectedColorFence.Id,
                            amount = 1,
                            special = "",
                            modifieddate = ConvertDateTime(DateTime.Now)
                        });
                        //sync change tabelle
                        con.Insert(new Mdh_Change
                        {
                            ctable = "houseconfig_has_attribute",
                            id = configId.houseconfig_id,
                            id2 = SelectedColorFence.Id,
                            change = "insert",
                            dt = ConvertDateTime(DateTime.Now),
                            synced = 0
                        });
                    }
                    con.Close();
                }
            }
            catch (Exception)
            {
                
            }
        }
        public void SQLCreateProject()
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    con.Insert(new Project
                    {
                        startdate = ConvertDateTime(DateTime.Now),
                        enddate = ConvertDateTime((DateTime.Now).AddYears(1)),
                        invoice = "Rechnung",
                        status = "1",
                        description = "",
                        modifieddate = ConvertDateTime(DateTime.Now),
                        customer_user_id = SelectedConfHouse.Customer.Id,
                        houseconfig_id = SelectedConfHouse.Id
                    });
                    int ProjectId = (con.Table<Project>().OrderByDescending(u => u.project_id).FirstOrDefault()).project_id;
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "project",
                        id = ProjectId,
                        change = "insert",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                    con.Update(new Houseconfig
                    {
                        houseconfig_id = SelectedConfHouse.Id,
                        price = (int)SelectedConfHouse.Price,
                        status = "0",
                        price_floor = 0,
                        modifieddate = ConvertDateTime(DateTime.Now),
                        house_package_id = SelectedConfHouse.Package.Id,
                        consultant_user_id = SelectedConfHouse.Consultant.Id,
                        customer_user_id = SelectedConfHouse.Customer.Id
                    });
                    //sync change tabelle
                    con.Insert(new Mdh_Change
                    {
                        ctable = "houseconfig",
                        id = SelectedConfHouse.Id,
                        change = "update",
                        dt = ConvertDateTime(DateTime.Now),
                        synced = 0
                    });
                }
            }
            catch (Exception)
            {
                
            }
        }

        //Get Attribute aus der Datenbank
        public List<DBModel.Attribute> SQLGetAttribute(int t)
        {
            List<DBModel.Attribute> models = new List<DBModel.Attribute>();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    models = (from c in con.Table<DBModel.Attribute>()
                              where c.attribute_group_id.Equals(t)
                              && c.deleted.Equals("0")
                              select c).ToList();

                    con.Close();
                }
            }
            catch (Exception)
            {
                
            }
            return models;
        }

        public List<ImageInherit> SQLGetHouses()
        {
            List<ImageInherit> house = new List<ImageInherit>();
            try
            {

                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    house = (from a in con.Table<Ymdh_House_Package>()
                             from c in con.Table<Ymdh_Address>()
                             where a.address_id.Equals(c.mdh_address_id)
                             select new ImageInherit
                             {
                                 Id = a.house_package_id,
                                 SourceImage = a.rootfolder,
                                 Description = a.description,
                                 Price = a.price,
                                 Zip = c.ZIP,
                                 City = c.City,
                                 Street = c.Street,
                                 HouseNo = c.houseno,
                                 Country = c.country
                             }).ToList();

                    con.Close();
                }
            }
            catch (Exception)
            {
                
            }
            return house;
        }
        public DBModel.Attribute SQLGetRightAttributeForHousePackage(int attributGroupId, SQLiteConnection con)
        {
            DBModel.Attribute i;

            i = (from a in con.Table<Package_Not_Attribute>()
                 from b in con.Table<DBModel.Attribute>()
                 where a.house_package_id.Equals(SelectedHouse.Id)
                 && b.attribute_id.Equals(a.attribute_id)
                 && b.attribute_group_id.Equals(attributGroupId)
                 select b).Single();
            return i;
        }
        public string ConvertDateTime(DateTime dt)
        {
            return String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt);
        }


        #endregion

        #region DrawSketches
        public RelayCommand ButtonSaveSketch { get; set; }
        public RelayCommand ButtonDeleteSketch { get; set; }
        public RelayCommand ButtonCancelSketch { get; set; }

        InkCanvas canvas;
        InkPresenter myPresenter;


        private void DrawSketch(int i)
        {
            GetFrame();
            a.Navigate(typeof(Pages.HKPages.GrundrissZeichnen));
            ButtonCancelSketch = new RelayCommand(ButtonCancelSketchMethod);
            if (i == 0)
            {
                ButtonSaveSketch = new RelayCommand(ButtonSaveSketchGroundfloorMethod);
                ButtonDeleteSketch = new RelayCommand(ButtonDeleteSketchGroundfloorMethod);
            }
            else if (i == 1)
            {
                ButtonSaveSketch = new RelayCommand(ButtonSaveSketchFloor1Method);
                ButtonDeleteSketch = new RelayCommand(ButtonDeleteSketchFloor1Method);
            }
            else if (i == 2)
            {
                ButtonSaveSketch = new RelayCommand(ButtonSaveSketchFloor2Method);
                ButtonDeleteSketch = new RelayCommand(ButtonDeleteSketchFloor2Method);
            }

            canvas = Pages.HKPages.GrundrissZeichnen.InkCanvasObject.GetInkCanvasObject();

            myPresenter = canvas.InkPresenter;
            myPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse
                | Windows.UI.Core.CoreInputDeviceTypes.Pen
                | Windows.UI.Core.CoreInputDeviceTypes.Touch;
        }
        //Save Sketch Methods
        private void ButtonSaveSketchGroundfloorMethod()
        {
            SaveSketchMehtod(0);
        }
        private void ButtonSaveSketchFloor1Method()
        {
            SaveSketchMehtod(1);
        }
        private void ButtonSaveSketchFloor2Method()
        {
            SaveSketchMehtod(2);
        }
        //Delete Sketch Methods
        private void ButtonDeleteSketchGroundfloorMethod()
        {
            ButtonDeleteSketchMethod(0);
        }
        private void ButtonDeleteSketchFloor1Method()
        {
            ButtonDeleteSketchMethod(1);
        }
        private void ButtonDeleteSketchFloor2Method()
        {
            ButtonDeleteSketchMethod(2);
        }
        //Save Sketch
        private async void SaveSketchMehtod(int i)
        {
            string name = Guid.NewGuid().ToString();
            var rootFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DreamHouse\\" + SelectedCustomerr.Id + "GroundPlots", CreationCollisionOption.OpenIfExists); // Create folder
            var coverpic_file = await rootFolder.CreateFileAsync(name + ".png", CreationCollisionOption.ReplaceExisting); // Create file
            string filePath = rootFolder.Path + "\\" + name + ".png";

            if (coverpic_file != null)
            {
                try
                {
                    using (IRandomAccessStream stream = await coverpic_file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await canvas.InkPresenter.StrokeContainer.SaveAsync(stream);
                    }
                    using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                    {
                        con.Insert(new Temp_Table()
                        {
                            description = name,
                            price = 0,
                            rootfolder = filePath
                        });
                        if (i == 0)
                        {
                            NewGroundPlotId0 = (con.Table<Temp_Table>().OrderByDescending(u => u.id).FirstOrDefault()).id;
                            FloorsGroundPlot.Add(new ImageInherit(filePath, 0, i, new RelayCommand(DrawSketchGroundfloorMethod), new RelayCommand(GroundfloorMethod), SelectedItemFloor, NumberOfFloorDB));
                        }
                        else if (i == 1)
                        {
                            NewGroundPlotId1 = (con.Table<Temp_Table>().OrderByDescending(u => u.id).FirstOrDefault()).id;
                            FloorsGroundPlot.Add(new ImageInherit(filePath, 0, i, new RelayCommand(DrawSketchFloor1Method), new RelayCommand(Floor1Method), SelectedItemFloor, NumberOfFloorDB));
                        }
                        else if (i == 2)
                        {
                            NewGroundPlotId2 = (con.Table<Temp_Table>().OrderByDescending(u => u.id).FirstOrDefault()).id;
                            FloorsGroundPlot.Add(new ImageInherit(filePath, 0, i, new RelayCommand(DrawSketchFloor2Method), new RelayCommand(Floor2Method), SelectedItemFloor, NumberOfFloorDB));
                        }
                        con.Close();
                    }
                    FloorsGroundPlot.RemoveAt(i);
                    FloorsGroundPlot.Move(FloorsGroundPlot.Count - 1, i);
                    ButtonForwardChoosePlotMethod();
                }
                catch (Exception)
                {
                }
            }
        }

        private void ButtonCancelSketchMethod()
        {
            ButtonForwardChoosePlotMethod();
        }

        private void ButtonDeleteSketchMethod(int i)
        {
            if (i == 0)
                DrawSketch(0);
            else if (i == 1)
                DrawSketch(1);
            else if (i == 2)
                DrawSketch(2);
        }

        #endregion

        #endregion

        #region UseCaseProjects
        //Liste vo Alle Porjecte Geladen werden
        #region Properties
        private List<Projects> listProjects = new List<Projects>();
        public List<Projects> ListProjects
        {
            get { return listProjects; }
            set { listProjects = value; OnChange("ListProjects"); }
        }

        public RelayCommand ButtonCreatePdfProjects { get; set; }

        //selected Project
        private Projects selectedProject;
        public Projects SelectedProject
        {
            get { return selectedProject; }
            set {
                selectedProject = value;
                SelectedProject.House = SQLGetHouseconfig2(SelectedProject.House);
                OnChange("SelectedProject");
                ProjectPezVisibility = "Visibible";
            }
        }
        private string projectPezVisibility = "Collapsed";

        public string ProjectPezVisibility
        {
            get { return projectPezVisibility; }
            set {
                if(selectedProject != null)
                {
                    projectPezVisibility = value;
                    OnChange("ProjectPezVisibility");
                }
                
            }
        }

        #endregion

        #region Methods
        public void ProjectsMethod()
        {
            ListProjects = SQLGetProject();
        }

        private async void ButtonCreatePdfProjectsMethod()
        {
            PdfDocument doc = new PdfDocument();

            //Add a page
            PdfPage page = doc.Pages.Add();


            PdfGraphics g = page.Graphics;
            //Set the format for string.
            PdfStringFormat format = new PdfStringFormat();
            //Set the alignment.
            format.Alignment = PdfTextAlignment.Center;

            //Create a solid brush //Set the font
            PdfBrush brush = new PdfSolidBrush(System.Drawing.Color.FromArgb(255, 0, 0, 0));
            PdfFont fontHeader = new PdfStandardFont(PdfFontFamily.Helvetica, 36);
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 30);
            //Draw the text
            //g.DrawString("PROJEKTDOKUMENTATION", font, brush, new System.Drawing.PointF(15, 200));
            //g.DrawString("Projekt 1", font, brush, new System.Drawing.PointF(15, 250), format);
            selectedConfHouse = SelectedProject.House;
            g.DrawString("PROJEKTDOKUMENTATION",
                fontHeader, brush, new System.Drawing.RectangleF(0, 300, page.GetClientSize().Width, page.GetClientSize().Height), format);
            g.DrawString("\n Projekt "+ selectedProject.Id +"\n Kosten: EUR "+ selectedProject.House.Price
                +"\nKunde: " + selectedProject.House.Customer.Name 
                +"\nStartdatum: " +selectedProject.StartDate
                +"\nEnddatum: "+selectedProject.EndDate
                +"\n Status: "+ selectedProject.StateDescription+" ", 
                font, brush, new System.Drawing.RectangleF(0, 350, page.GetClientSize().Width, page.GetClientSize().Height), format);

            
            CreatePdfMethod(doc);

            //MemoryStream stream = new MemoryStream();
            //await doc.SaveAsync(stream);
            //doc.Close(true);
            //Save(stream, "Projekt.pdf");

        }

        #endregion

        #region SQL

        public List<Projects> SQLGetProject()
        {
            List<Projects> p = new List<Projects>();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    p = (from c in con.Table<Project>()
                         from d in con.Table<Houseconfig>()
                         where c.status.Equals("1")
                         && c.houseconfig_id.Equals(d.houseconfig_id)
                         select new Projects()
                         {
                             Id = c.project_id,
                             StartDate = ConvertDateTime(DateTime.Parse(c.startdate)),
                             EndDate = ConvertDateTime(DateTime.Parse(c.enddate)),
                             State = c.status,
                             Description = c.description,
                             House = (from a in con.Table<Mdh_Users>()
                                      from f in con.Table<Ymdh_House_Package>()
                                      from c in con.Table<Ymdh_Address>()
                                      where a.id.Equals(d.customer_user_id)
                                      && d.house_package_id.Equals(f.house_package_id)
                                      && f.address_id.Equals(c.mdh_address_id)
                                      select new HouseSummary
                                      {
                                          Id = d.houseconfig_id,
                                          Price = d.price,
                                          ConfDate = d.modifieddate,
                                          Customer = new Customer(a.id, a.name, SQLCustomerCountProject(a.id, con), SQLCustomerCountHouseconfig(a.id, con)),
                                          Consultant = new Consultant(d.consultant_user_id, SQLGetConsultantName(d.consultant_user_id, con)),
                                          Package = new ImageInherit(f.rootfolder, f.house_package_id, f.description, f.price, c.ZIP, c.City, c.Street, c.houseno, c.country)
                                      }).Single()
                         }).ToList();

                    con.Close();
                }
            }
            catch (Exception)
            {

            }
            return p;
        }

        #endregion

        #endregion

        #region UseCase ManageAppointment
        #region Properties

        //Datum auswählen 
        private DateTime dateAppointment;

        public DateTime DateAppointment
        {
            get { return dateAppointment; }
            set { dateAppointment = value; OnChange("DateAppointment"); }
        }


        //Zeit auswählen
        private TimeSpan timeAppoitment;

        public TimeSpan TimeAppoitment
        {
            get { return timeAppoitment; }
            set { timeAppoitment = value; OnChange("TimeAppoitment"); }
        }

        //MinYear
        private DateTime dateTimeNow = DateTime.Now;

        public DateTime DateTimeNow
        {
            get { return dateTimeNow; }
            set { dateTimeNow = value; OnChange("DateTimeNow"); }
        }


        private List<Appointment> appointments = new List<Appointment>();

        public List<Appointment> Appointments
        {
            get { return appointments; }
            set { appointments = value; OnChange("Appointments"); }
        }

        private ObservableCollection<Consultant> consultants = new ObservableCollection<Consultant>();

        public ObservableCollection<Consultant> Consultants
        {
            get { return consultants; }
            set { consultants = value; }
        }


        //Selected
        private Appointment selectedAppointment;

        public Appointment SelectedAppointment
        {
            get { return selectedAppointment; }
            set { selectedAppointment = value; OnChange("SelectedAppointment"); }
        }


        private Consultant selectedConsultant;

        public Consultant SelectedConsultant
        {
            get { return selectedConsultant; }
            set { selectedConsultant = value; OnChange("SelectedConsultant"); }
        }


        public RelayCommand AddNewAppointmentButton { get; set; }
        public RelayCommand ButtonBackToAppointmentPage { get; set; }
        public RelayCommand ButtonSaveAppointment { get; set; }
        public RelayCommand EditAppointmentButton { get; set; }
        public RelayCommand ButtonSaveEditedAppointment { get; set; }
        public RelayCommand DeleteAppointmentButton { get; set; }

        #endregion

        #region Methods
        //In dieser Methode werden alle Buttons die für den ManageAppointment UseCase gebraucht werden
        //initialisiert
        public void ManageAppointments()
        {
            AddNewAppointmentButton = new RelayCommand(AddNewAppointmentButtonMethod);
            ButtonBackToAppointmentPage = new RelayCommand(ButtonBackToAppointmentPageMethod);
            ButtonSaveAppointment = new RelayCommand(ButtonSaveAppointmentMethod);
            EditAppointmentButton = new RelayCommand(EditAppointmentButtonMethod);
            ButtonSaveEditedAppointment = new RelayCommand(ButtonSaveEditedAppointmentMethod);
            DeleteAppointmentButton = new RelayCommand(DeleteAppointmentButtonMethod);

            LoadAppointments();
        }

        private async void DeleteAppointmentButtonMethod()
        {
            if (SelectedAppointment != null)
            {
                try
                {
                    using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                    {
                        var p = (from a in con.Table<Ymdh_Appointment>()
                                 where a.appointment_id.Equals(SelectedAppointment.Id)
                                 select a).ToList();
                        p[0].appointment_status_id = 2;

                        MessageDialog msgDialog = new MessageDialog("Sind Sie sicher, dass Sie diesen Termin (" + SelectedAppointment.DateFormat + " " + SelectedAppointment.Time +
                            " " + SelectedAppointment.Customer.Name + SelectedAppointment.Consultant.Name + ") löschen wollen?");
                        UICommand yesCmd = new UICommand("Ja");
                        msgDialog.Commands.Add(yesCmd);
                        UICommand noCmd = new UICommand("Nein");
                        msgDialog.Commands.Add(noCmd);
                        IUICommand cmd = await msgDialog.ShowAsync();
                        if (cmd == yesCmd)
                        {
                            //Update funktion
                            con.Update(new Ymdh_Appointment()
                            {
                                appointment_id = p[0].appointment_id,
                                appointment_status_id = p[0].appointment_status_id,
                                consultant_user_id = p[0].consultant_user_id,
                                from_ = p[0].from_,
                                house_package_id = p[0].house_package_id,
                                message_id = p[0].message_id,
                                user_id = p[0].user_id
                            });
                            //sync change tabelle
                            con.Insert(new Mdh_Change
                            {
                                ctable = "ymdh_appointment",
                                id = p[0].appointment_id,
                                change = "delete",
                                dt = ConvertDateTime(DateTime.Now),
                                synced = 0
                            });

                            var dialog = new MessageDialog("Der ausgewählte Termin wurde gelöscht.");
                            await dialog.ShowAsync();
                            Appointments.Clear();
                            LoadAppointments();
                        }
                        con.Close();
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie einen Termin aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier werden alle bereits festgelegten Termine angezeigt
        public void LoadAppointments()
        {
            List<Appointment> List;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    List = (from a in con.Table<Ymdh_Appointment>()
                            from b in con.Table<Ymdh_Message>()
                            from c in con.Table<Mdh_Users>()
                            from d in con.Table<Mdh_User_Usergroup_Map>()
                            from e in con.Table<Mdh_Usergroups>()
                            where a.message_id == b.message_id
                            && a.user_id == c.id
                            && c.id == d.user_id
                            && d.group_id == e.id
                            && a.appointment_status_id == 0
                            orderby a.from_
                            select new Appointment()
                            {
                                Id = a.appointment_id,
                                Date = DateTime.Parse(a.from_.Substring(0,10)),
                                Time = TimeSpan.Parse(a.from_.Substring(11)),
                                Customer = new Customer(c.id, c.name,0,0),
                                Consultant = GetConsultantForAppointment(con, a),
                                Message = b.message
                            }).ToList();

                    con.Close();
                }
                Appointments = List;
            }
            catch (Exception)
            {

            }
        }
        private Consultant GetConsultantForAppointment(SQLiteConnection con, Ymdh_Appointment Appointment)
        {
            Consultant consultant;
            consultant = (from b in con.Table<Ymdh_Message>()
                    from c in con.Table<Mdh_Users>()
                    from d in con.Table<Mdh_User_Usergroup_Map>()
                    from e in con.Table<Mdh_Usergroups>()
                    where Appointment.message_id == b.message_id
                    && Appointment.consultant_user_id == c.id
                    && c.id == d.user_id
                    && d.group_id == e.id
                    orderby Appointment.from_
                    select new Consultant()
                    {
                        Id = c.id,
                        Name = c.name,
                        Username = c.username
                    }).Single();
            return consultant;
        }

        private async void EditAppointmentButtonMethod()
        {
            if (selectedAppointment != null)
            {
                GetFrame();
                a.Navigate(typeof(Pages.TerminePages.TerminBearbeiten));

                SQLGetConsultants();

                SelectedCustomerr = SelectedAppointment.Customer;
                SelectedConsultant = SelectedAppointment.Consultant;
                DateAppointment = SelectedAppointment.Date;
                TimeAppoitment = SelectedAppointment.Time;
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie einen Termin aus.");
                await dialog.ShowAsync();
            }
        }

        private async void ButtonSaveEditedAppointmentMethod()
        {
            //checken ob Kunde und Consultant ausgewählt wurden
            if (SelectedCustomerr != null && SelectedConsultant != null)
            {
                try
                {
                    string pattern = "yyyy-MM-dd HH:mm";

                    DateAppointment = dateAppointment.AddHours(-DateTimeNow.Hour);
                    dateAppointment = dateAppointment.AddMinutes(-DateTimeNow.Minute);

                    DateTime choosenAppointment = DateAppointment + TimeAppoitment;

                    string dt = choosenAppointment.ToString(pattern, CultureInfo.CurrentUICulture);

                    //checken ob der termin für den CONSULTANT belegt ist
                    List<Ymdh_Appointment> models;

                    using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                    {
                        models = (from c in con.Table<Ymdh_Appointment>()
                                  where c.consultant_user_id.Equals(SelectedConsultant.Id)
                                  && c.from_.Equals(dt)
                                  && c.appointment_status_id.Equals(0)
                                  select c).ToList();

                        con.Close();
                    }
                    //checken ob der termin für den KUNDEN belegt ist
                    List<Ymdh_Appointment> models2;

                    using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                    {
                        models2 = (from c in con.Table<Ymdh_Appointment>()
                                   where c.user_id.Equals(SelectedCustomerr.Id)
                                   && c.from_.Equals(dt)
                                   && c.appointment_status_id.Equals(0)
                                   select c).ToList();

                        con.Close();
                    }
                    if (models.Count > 0 || models2.Count > 0)
                    {
                        var dialog1 = new MessageDialog("Sie haben bereits am " + models[0].from_ + " einen Termin!");
                        await dialog1.ShowAsync();
                    }
                    else
                    {
                        if (DateAppointment.DayOfWeek.ToString() == "Sunday")
                        {
                            var dialog1 = new MessageDialog("Dieser Termin ist ein Sonntag! Bitte wählen Sie ein anderes Datum aus.");
                            await dialog1.ShowAsync();
                        }
                        else if (DateAppointment.DayOfWeek.ToString() == "Saturday")
                        {
                            var dialog1 = new MessageDialog("Dieser Termin ist ein Samstag! Bitte wählen Sie ein anderes Datum aus.");
                            await dialog1.ShowAsync();
                        }
                        else
                        {
                            //in die DB speichern
                            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                            {
                                con.Update(new Ymdh_Appointment()
                                {
                                    appointment_id = selectedAppointment.Id,
                                    appointment_status_id = 0,
                                    consultant_user_id = SelectedConsultant.Id,
                                    from_ = dt,
                                    user_id = SelectedCustomerr.Id
                                });
                                //sync change tabelle
                                con.Insert(new Mdh_Change
                                {
                                    ctable = "ymdh_appointment",
                                    id = SelectedAppointment.Id,
                                    change = "update",
                                    dt = ConvertDateTime(DateTime.Now),
                                    synced = 0
                                });
                                con.Close();
                            }
                            var dialog1 = new MessageDialog("Ihr Termin für " + dt + " wurde gespeichert!");
                            await dialog1.ShowAsync();
                            ButtonBackToAppointmentPageMethod();
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                var dialog1 = new MessageDialog("Bitte wählen Sie einen Kunden und einen Mitarbeiter aus!");
                await dialog1.ShowAsync();
            }
        }

        private async void ButtonSaveAppointmentMethod()
        {
            //checken ob Kunde und Consultant ausgewählt wurden
            if (SelectedCustomerr != null && SelectedConsultant != null)
            {
                try
                {
                    string pattern = "yyyy-MM-dd HH:mm";

                    DateAppointment = dateAppointment.AddHours(-DateTimeNow.Hour);
                    dateAppointment = dateAppointment.AddMinutes(-DateTimeNow.Minute);

                    DateTime choosenAppointment = DateAppointment + TimeAppoitment;

                    string dt = choosenAppointment.ToString(pattern, CultureInfo.CurrentUICulture);

                    //checken ob der termin für den CONSULTANT belegt ist
                    List<Ymdh_Appointment> models;

                    using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                    {
                        models = (from c in con.Table<Ymdh_Appointment>()
                                  where c.consultant_user_id.Equals(SelectedConsultant.Id)
                                  && c.from_.Equals(dt)
                                  && c.appointment_status_id.Equals(0)
                                  select c).ToList();

                        con.Close();
                    }
                    //checken ob der termin für den KUNDEN belegt ist
                    List<Ymdh_Appointment> models2;

                    using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                    {
                        models2 = (from c in con.Table<Ymdh_Appointment>()
                                   where c.user_id.Equals(SelectedCustomerr.Id)
                                   && c.from_.Equals(dt)
                                   && c.appointment_status_id.Equals(0)
                                   select c).ToList();

                        con.Close();
                    }
                    if (models.Count > 0 || models2.Count > 0)
                    {
                        var dialog1 = new MessageDialog("Sie haben bereits am " + models[0].from_ + " einen Termin!");
                        await dialog1.ShowAsync();
                    }
                    else
                    {
                        if (DateAppointment.DayOfWeek.ToString() == "Sunday")
                        {
                            var dialog1 = new MessageDialog("Dieser Termin ist ein Sonntag! Bitte wählen Sie ein anderes Datum aus.");
                            await dialog1.ShowAsync();
                        }
                        else if (DateAppointment.DayOfWeek.ToString() == "Saturday")
                        {
                            var dialog1 = new MessageDialog("Dieser Termin ist ein Samstag! Bitte wählen Sie ein anderes Datum aus.");
                            await dialog1.ShowAsync();
                        }
                        else
                        {
                            //in die DB speichern
                            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                            {
                                var AppointId = (con.Table<Ymdh_Appointment>().OrderByDescending(u => u.appointment_id).FirstOrDefault()).appointment_id;
                                con.Insert(new Ymdh_Appointment()
                                {
                                    appointment_id = AppointId + 1,
                                    appointment_status_id = 0,
                                    consultant_user_id = SelectedConsultant.Id,
                                    from_ = dt,
                                    user_id = SelectedCustomerr.Id
                                });
                                int AppointmentId = (con.Table<Ymdh_Appointment>().OrderByDescending(u => u.appointment_id).FirstOrDefault()).appointment_id;
                                //sync change tabelle
                                con.Insert(new Mdh_Change
                                {
                                    ctable = "ymdh_appointment",
                                    id = AppointmentId,
                                    change = "insert",
                                    dt = ConvertDateTime(DateTime.Now),
                                    synced = 0
                                });
                                con.Close();
                            }
                            var dialog1 = new MessageDialog("Ihr Termin für " + dt + " wurde gespeichert!");
                            await dialog1.ShowAsync();
                            ButtonBackToAppointmentPageMethod();
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                var dialog1 = new MessageDialog("Bitte wählen Sie einen Kunden und einen Mitarbeiter aus!");
                await dialog1.ShowAsync();
            }
        }

        private void ButtonBackToAppointmentPageMethod()
        {
            GetFrame();
            a.Navigate(typeof(Pages.TerminePage));
            Appointments.Clear();
            LoadAppointments();
            UserAppointments.Clear();
            LoadUserAppointments();
        }

        //Neuen Termin erstellen
        public void AddNewAppointmentButtonMethod()
        {
            SQLGetConsultants();
            DateAppointment = DateTime.Now;
            TimeAppoitment = DateTime.Now.TimeOfDay;
            GetFrame();
            a.Navigate(typeof(Pages.TerminePages.NeuTermin));
        }

        public void SQLGetConsultants()
        {
            Consultants.Clear();
            List<Mdh_Users> model;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    model = (from a in con.Table<Mdh_Users>()
                             from b in con.Table<Mdh_User_Usergroup_Map>()
                             from c in con.Table<Mdh_Usergroups>()
                             where a.id.Equals(b.user_id)
                             && b.group_id.Equals(c.id)
                             && c.title.Equals("Consultant")
                             select a).ToList();

                    con.Close();
                }
                for (int i = 0; i < model.Count; i++)
                {
                    Consultants.Add(new Consultant()
                    {
                        Id = model[i].id,
                        //Firstname = model[i].name,
                        Name = model[i].name,
                    });
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion
        #endregion

        #region UseCase CreatePdf

        #region Properties
        private ObservableCollection<HouseSummary> listConfHouses = new ObservableCollection<HouseSummary>();

        public ObservableCollection<HouseSummary> ListConfHouses
        {
            get { return listConfHouses; }
            set { listConfHouses = value;}
        }
        private HouseSummary selectedConfHouse;

        public HouseSummary SelectedConfHouse
        {
            get { return selectedConfHouse; }
            set {
                selectedConfHouse = value;
                OnChange("SelectedConfHouse");
                PDFButtonVisibility = "Visible";
            }
        }


        public RelayCommand ButtonCreatePdf { get; set; }
        #endregion

        #region Methods

        public void CreatePdf()
        {
            listConfHouses.Clear();
            //ButtonCreatePdf = new RelayCommand(ButtonCreatePdfMethod);
            foreach (var item in SQLGetHouseconfig())
            {
                ListConfHouses.Add(item);
            }
        }

        private async void CreatePdfMethod(PdfDocument d)
        {
            if (SelectedConfHouse != null)
            {
                //Create a new document.
                PdfDocument doc = d;
                PdfPage page = doc.Pages.Add();

                #region Header
                //Set the font
                PdfBrush brushHeader = new PdfSolidBrush(System.Drawing.Color.FromArgb(255, 0, 0, 0));
                PdfFont fontHeader = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);

                //Create a header and draw the image.
                System.Drawing.RectangleF bounds = new System.Drawing.RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 60);
                PdfPageTemplateElement header = new PdfPageTemplateElement(bounds);
                SaveLogo(header);


                header.Graphics.DrawString(COMPANY_DESC, fontHeader, brushHeader, new System.Drawing.PointF(0, 0));
                header.Graphics.DrawLine(new PdfPen(new PdfColor(212, 0, 73), 3), new System.Drawing.PointF(0, 55), new System.Drawing.PointF(600, 55));
                //Add the header at the top.
                doc.Template.Top = header;
                #endregion

                //Create Pdf graphics for the page
                PdfGraphics g = page.Graphics;


                //Create a solid brush //Set the font
                PdfBrush brushHeadLine = new PdfSolidBrush(System.Drawing.Color.FromArgb(255, 0, 0, 0));
                PdfFont fontHeadLine = new PdfStandardFont(PdfFontFamily.Helvetica, 16);
                //Draw the text
                g.DrawString("SCHNELL ZUM TRAUMHAUS\nGANZ EINFACH MIT UNSEREM HAUSKONFIGURATOR", fontHeadLine, brushHeadLine, new System.Drawing.PointF(0, 5));

                //Name Haus Preis Date
                PdfFont font1 = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
                PdfBrush brush1 = new PdfSolidBrush(System.Drawing.Color.FromArgb(255, 0, 0, 0));
                g.DrawString("Name: " + SelectedConfHouse.Customer.Name, font1, brush1, new System.Drawing.PointF(0, 45));
                g.DrawString("Preis: " + SelectedConfHouse.Price, font1, brush1, new System.Drawing.PointF(0, 58));
                g.DrawString("Haus: " + SelectedConfHouse.Package.Description, font1, brush1, new System.Drawing.PointF(0, 71));
                g.DrawString("Datum: " + SelectedConfHouse.ConfDate, font1, brush1, new System.Drawing.PointF(0, 84));


                #region DrawImages
                HouseconfigFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DreamHouse\\Houseconfig\\", CreationCollisionOption.OpenIfExists); // Create folder
                                                                                                                                            //string imagePath = folder + "\\" + parts[10] + "\\" + parts[11];
                SaveImage(page, SelectedConfHouse.Package.SourceImage, 400, 230, 60, 110);

                //Plot
                PdfFont font2 = new PdfStandardFont(PdfFontFamily.Helvetica, 14);
                PdfBrush brush2 = new PdfSolidBrush(System.Drawing.Color.FromArgb(255, 0, 0, 0));
                g.DrawString("Grundstück: " + SelectedConfHouse.Plot.Description, fontHeadLine, brushHeadLine, new System.Drawing.PointF(0, 360));
                g.DrawString("Preis: EUR " + SelectedConfHouse.Plot.Price, font2, brush2, new System.Drawing.PointF(0, 380));

                SaveImage(page, SelectedConfHouse.Plot.SourceImage, 350, 220, 10, 400);

                //2nd Page
                PdfPage page2 = doc.Pages.Add();
                g = page2.Graphics;


                //GroundPlots
                g.DrawString("Grundrisse der Stockwerke ", fontHeadLine, brushHeadLine, new System.Drawing.PointF(0, 5));

                if (SelectedConfHouse.GroundPlots != null)
                {
                    int i = 0;
                    string floor = "";
                    int distance = 30;
                    foreach (var Groundis in SelectedConfHouse.GroundPlots)
                    {
                        if (i == 0) floor = "Erdgeschoss";
                        else if (i == 1) floor = "1. Stockwerk";
                        else floor = "2. Stockwerk";

                        SaveImage(page2, Groundis.SourceImage, 330, 220, 10, distance);
                        g.DrawString(floor, font2, brush2, new System.Drawing.PointF(350, distance));
                        distance += 230;
                        i++;

                    }
                }

                //3rd Page
                PdfPage page3 = doc.Pages.Add();
                g = page3.Graphics;
                //OutsideWALL
                g.DrawString("Außenwand: " + SelectedConfHouse.OutsideWall.Description, fontHeadLine, brushHeadLine, new System.Drawing.PointF(0, 5));
                g.DrawString("Preis: EUR " + SelectedConfHouse.OutsideWall.Price, font2, brush2, new System.Drawing.PointF(0, 24));

                SaveImage(page3, SelectedConfHouse.OutsideWall.SourceImage, 400, 230, 10, 42);
                if (SelectedConfHouse.OutsideWallColor != null)
                {
                    g.DrawRectangle(new PdfSolidBrush(
                    System.Drawing.Color.FromArgb(255,
                    selectedConfHouse.OutsideWallColor.R_byte,
                    selectedConfHouse.OutsideWallColor.G_byte,
                    selectedConfHouse.OutsideWallColor.B_byte)), new System.Drawing.RectangleF(420, 42, 50, 50));
                }


                //InsideWALL
                g.DrawString("Innenwand: " + SelectedConfHouse.InsideWall.Description, fontHeadLine, brushHeadLine, new System.Drawing.PointF(0, 287));
                g.DrawString("Preis: EUR " + SelectedConfHouse.InsideWall.Price, font2, brush2, new System.Drawing.PointF(0, 306));
                SaveImage(page3, SelectedConfHouse.InsideWall.SourceImage, 400, 230, 10, 324);
                if (SelectedConfHouse.InsideWallColor != null)
                {
                    g.DrawRectangle(new PdfSolidBrush(
                    System.Drawing.Color.FromArgb(255,
                    SelectedConfHouse.InsideWallColor.R_byte,
                    SelectedConfHouse.InsideWallColor.G_byte,
                    SelectedConfHouse.InsideWallColor.B_byte)), new System.Drawing.RectangleF(420, 324, 50, 50));
                }



                //4th Page
                PdfPage page4 = doc.Pages.Add();
                g = page4.Graphics;
                //Rooftype
                g.DrawString("Dachtyp: " + SelectedConfHouse.RoofType.Description, fontHeadLine, brushHeadLine, new System.Drawing.PointF(0, 5));
                g.DrawString("Preis: EUR " + SelectedConfHouse.RoofType.Price, font2, brush2, new System.Drawing.PointF(0, 24));
                SaveImage(page4, SelectedConfHouse.RoofType.SourceImage, 400, 230, 10, 42);

                //Roofmaterial
                g.DrawString("Dachmaterial: " + SelectedConfHouse.RoofMaterial.Description, fontHeadLine, brushHeadLine, new System.Drawing.PointF(0, 287));
                g.DrawString("Preis: EUR " + SelectedConfHouse.RoofMaterial.Price, font2, brush2, new System.Drawing.PointF(0, 306));
                SaveImage(page4, SelectedConfHouse.RoofMaterial.SourceImage, 400, 230, 10, 324);


                //5th Page
                PdfPage page5 = doc.Pages.Add();
                g = page5.Graphics;
                //Window
                g.DrawString("Fenster" + SelectedConfHouse.Window.Description, fontHeadLine, brushHeadLine, new System.Drawing.PointF(0, 5));
                g.DrawString("Preis: EUR " + SelectedConfHouse.Window.Price, font2, brush2, new System.Drawing.PointF(0, 24));

                SaveImage(page5, SelectedConfHouse.Window.SourceImage, 400, 230, 10, 42);
                if (SelectedConfHouse.WindowColor != null)
                {
                    g.DrawRectangle(new PdfSolidBrush(
                    System.Drawing.Color.FromArgb(255,
                    SelectedConfHouse.WindowColor.R_byte,
                    SelectedConfHouse.WindowColor.G_byte,
                    SelectedConfHouse.WindowColor.B_byte)), new System.Drawing.RectangleF(420, 42, 50, 50));
                }


                //Doors
                g.DrawString("Türen: " + SelectedConfHouse.Door.Description, fontHeadLine, brushHeadLine, new System.Drawing.PointF(0, 287));
                g.DrawString("Preis: EUR " + SelectedConfHouse.Door.Price, font2, brush2, new System.Drawing.PointF(0, 306));
                SaveImage(page5, SelectedConfHouse.Door.SourceImage, 400, 230, 10, 324);
                if (SelectedConfHouse.DoorColor != null)
                {
                    g.DrawRectangle(new PdfSolidBrush(
                    System.Drawing.Color.FromArgb(255,
                    SelectedConfHouse.DoorColor.R_byte,
                    SelectedConfHouse.DoorColor.G_byte,
                    SelectedConfHouse.DoorColor.B_byte)), new System.Drawing.RectangleF(420, 324, 50, 50));
                }



                //6th Page
                PdfPage page6 = doc.Pages.Add();
                g = page6.Graphics;
                //Kamin
                g.DrawString("Kamin: " + SelectedConfHouse.Chimney.Description, fontHeadLine, brushHeadLine, new System.Drawing.PointF(0, 5));
                g.DrawString("Preis: EUR " + SelectedConfHouse.Chimney.Price, font2, brush2, new System.Drawing.PointF(0, 24));
                SaveImage(page6, SelectedConfHouse.Chimney.SourceImage, 400, 230, 10, 42);

                //Steckdosen pro Raum
                g.DrawString("Steckdosen pro Raum: " + SelectedConfHouse.NumberOfSocket, font2, brush2, new System.Drawing.PointF(0, 324));

                //Energiesystem
                g.DrawString("Energiesystem: " + SelectedConfHouse.EnergySystem.Name, font2, brush2, new System.Drawing.PointF(0, 345));
                g.DrawString("Preis: EUR " + SelectedConfHouse.EnergySystem.Price, font2, brush2, new System.Drawing.PointF(0, 360));

                //Heizungssystem
                g.DrawString("Heizungssystem: " + SelectedConfHouse.HeatingSystem.Name, font2, brush2, new System.Drawing.PointF(0, 378));
                g.DrawString("Preis: EUR " + SelectedConfHouse.HeatingSystem.Price, font2, brush2, new System.Drawing.PointF(0, 393));

                //Garage
                g.DrawString("Garage: " + SelectedConfHouse.Garage.Name, font2, brush2, new System.Drawing.PointF(0, 414));


                //7th Page
                PdfPage page7 = doc.Pages.Add();
                g = page7.Graphics;
                //Pool
                g.DrawString("Swimmingpool: " + SelectedConfHouse.Pool.Description, fontHeadLine, brushHeadLine, new System.Drawing.PointF(0, 5));
                g.DrawString("Preis: EUR " + SelectedConfHouse.Pool.Price, font2, brush2, new System.Drawing.PointF(0, 24));

                SaveImage(page7, SelectedConfHouse.Pool.SourceImage, 400, 230, 10, 42);
                g.DrawString("Größe: " + SelectedConfHouse.Poolsize + " m²", font2, brush2, new System.Drawing.PointF(420, 42));

                //Zaun
                g.DrawString("Zaun: " + SelectedConfHouse.Fence.Description, fontHeadLine, brushHeadLine, new System.Drawing.PointF(0, 287));
                g.DrawString("Preis: EUR " + SelectedConfHouse.Fence.Price, font2, brush2, new System.Drawing.PointF(0, 306));
                SaveImage(page7, SelectedConfHouse.Fence.SourceImage, 400, 230, 10, 324);
                if (SelectedConfHouse.FenceColor != null)
                {
                    g.DrawRectangle(new PdfSolidBrush(
                    System.Drawing.Color.FromArgb(255,
                    SelectedConfHouse.FenceColor.R_byte,
                    SelectedConfHouse.FenceColor.G_byte,
                    SelectedConfHouse.FenceColor.B_byte)), new System.Drawing.RectangleF(420, 324, 50, 50));
                }

                #endregion

                #region GRID

                #endregion

                MemoryStream stream = new MemoryStream();
                await doc.SaveAsync(stream);
                doc.Close(true);
                Save(stream, "ConfHouse.pdf");

            }
        }
        private void ButtonCreatePdfMethod()
        {
            SelectedConfHouse = SQLGetHouseconfig2(SelectedConfHouse);
            CreatePdfMethod(new PdfDocument());
        }

        async void SaveImage(PdfPage page, string path, int width, int height, float x, float y)
        {
            PdfGraphics graphics = page.Graphics;
            Stream imageStream = File.OpenRead(@path);
            //Load the image from the disk.

            PdfBitmap image = new PdfBitmap(imageStream);
            //Draw the image

            graphics.DrawImage(image, x, y, width, height);
        }
        async void SaveLogo(PdfPageTemplateElement header)
        {
            Stream imageStream = File.OpenRead("Bilder\\Logo\\DreamHouse_lang.png");
            PdfBitmap image = new PdfBitmap(imageStream);
            header.Graphics.DrawImage(image, new System.Drawing.PointF(400, 0), new System.Drawing.SizeF(150, 50));
        }
        async void Save(Stream stream, string filename)
        {
            stream.Position = 0;

            StorageFile stFile;
            if (!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")))
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.DefaultFileExtension = ".pdf";
                savePicker.SuggestedFileName = filename;
                savePicker.FileTypeChoices.Add("Adobe PDF Document", new List<string>() { ".pdf" });
                stFile = await savePicker.PickSaveFileAsync();
            }
            else
            {
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                stFile = await local.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            }
            if (stFile != null)
            {
                Windows.Storage.Streams.IRandomAccessStream fileStream = await stFile.OpenAsync(FileAccessMode.ReadWrite);
                Stream st = fileStream.AsStreamForWrite();
                st.SetLength(0);
                st.Write((stream as MemoryStream).ToArray(), 0, (int)stream.Length);
                st.Flush();
                st.Dispose();
                fileStream.Dispose();
                MessageDialog msgDialog = new MessageDialog("Do you want to view the Document?", "File has been created successfully.");
                UICommand yesCmd = new UICommand("Yes");
                msgDialog.Commands.Add(yesCmd);
                UICommand noCmd = new UICommand("No");
                msgDialog.Commands.Add(noCmd);
                IUICommand cmd = await msgDialog.ShowAsync();
                if (cmd == yesCmd)
                {
                    // Launch the retrieved file
                    bool success = await Windows.System.Launcher.LaunchFileAsync(stFile);
                }
            }
        }
        #endregion

        #region SQL
        private int SQLCustomerCountProject(int i, SQLiteConnection con)
        {
            List<Project> p;
            int h;
            p = (from a in con.Table<Houseconfig>()
                 from b in con.Table<Project>()
                 where a.customer_user_id.Equals(i)
                 && b.houseconfig_id.Equals(a.houseconfig_id)
                 && b.status.Equals("1")
                 select b).ToList();
            h = p.Count;

            return h;
        }

        private int SQLCustomerCountHouseconfig(int i, SQLiteConnection con)
        {
            List<Houseconfig> p;
            int h;
            p = (from a in con.Table<Houseconfig>()
                 where a.customer_user_id.Equals(i)
                 && a.status.Equals("1")
                 select a).ToList();
            h = p.Count;

            return h;
        }

        private string SQLGetConsultantName(int id, SQLiteConnection con)
        {
            string name;
            name = (from a in con.Table<Mdh_Users>()
                    where a.id.Equals(id)
                    select a.name).Single();

            return name;
        }

        private HouseSummary SQLGetHouseconfig2(HouseSummary SelectedHouse)
        {
            HouseSummary house = new HouseSummary();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    house = (from x in con.Table<Houseconfig>()
                             where x.houseconfig_id.Equals(SelectedHouse.Id)
                             select new HouseSummary
                             {
                                 Id = SelectedHouse.Id,
                                 Customer = SelectedHouse.Customer,
                                 Consultant = SelectedHouse.Consultant,
                                 ConfDate = SelectedHouse.ConfDate,
                                 Package = SelectedHouse.Package,
                                 Price = SelectedHouse.Price,
                                 Plot = (from a in con.Table<Houseconfig_Has_Attribute>()
                                         from b in con.Table<DBModel.Attribute>()
                                         where a.houseconfig_id.Equals(SelectedHouse.Id)
                                         && a.attribute_id.Equals(b.attribute_id)
                                         && b.attribute_group_id.Equals(3)
                                         select new ImageInherit()
                                         {
                                             SourceImage = b.rootfolder,
                                             Id = b.attribute_id,
                                             Description = b.description,
                                             Price = b.price
                                         }).FirstOrDefault(),
                                 numberOfFloors = int.Parse((from a in con.Table<Housefloor>()
                                                             where a.houseconfig_id.Equals(SelectedHouse.Id)
                                                             select a.area).LastOrDefault().ToString()),
                                 GroundPlots = SQLGetGroundPlots(SelectedHouse.Id, con),
                                 OutsideWall = (from a in con.Table<Houseconfig_Has_Attribute>()
                                                from b in con.Table<DBModel.Attribute>()
                                                where a.houseconfig_id.Equals(SelectedHouse.Id)
                                                && a.attribute_id.Equals(b.attribute_id)
                                                && b.attribute_group_id.Equals(5)
                                                select new ImageInherit()
                                                {
                                                    SourceImage = b.rootfolder,
                                                    Id = b.attribute_id,
                                                    Description = b.description,
                                                    Price = b.price
                                                }).FirstOrDefault(),
                                 OutsideWallColor = (from a in con.Table<Houseconfig_Has_Attribute>()
                                                     from b in con.Table<DBModel.Attribute>()
                                                     where a.houseconfig_id.Equals(SelectedHouse.Id)
                                                     && a.attribute_id.Equals(b.attribute_id)
                                                     && b.attribute_group_id.Equals(902)
                                                     select new ColorPalette
                                                     (b.attribute_id, Byte.Parse(b.description.Split(',').GetValue(0).ToString()), Byte.Parse(b.description.Split(',').GetValue(1).ToString()), Byte.Parse(b.description.Split(',').GetValue(2).ToString()))).FirstOrDefault(),
                                 InsideWall = (from a in con.Table<Houseconfig_Has_Attribute>()
                                               from b in con.Table<DBModel.Attribute>()
                                               where a.houseconfig_id.Equals(SelectedHouse.Id)
                                               && a.attribute_id.Equals(b.attribute_id)
                                               && b.attribute_group_id.Equals(51)
                                               select new ImageInherit()
                                               {
                                                   SourceImage = b.rootfolder,
                                                   Id = b.attribute_id,
                                                   Description = b.description,
                                                   Price = b.price
                                               }).FirstOrDefault(),
                                 InsideWallColor = (from a in con.Table<Houseconfig_Has_Attribute>()
                                                    from b in con.Table<DBModel.Attribute>()
                                                    where a.houseconfig_id.Equals(SelectedHouse.Id)
                                                    && a.attribute_id.Equals(b.attribute_id)
                                                    && b.attribute_group_id.Equals(901)
                                                    select new ColorPalette
                                                    (b.attribute_id, Byte.Parse(b.description.Split(',').GetValue(0).ToString()), Byte.Parse(b.description.Split(',').GetValue(1).ToString()), Byte.Parse(b.description.Split(',').GetValue(2).ToString()))).FirstOrDefault(),
                                 RoofType = (from a in con.Table<Houseconfig_Has_Attribute>()
                                             from b in con.Table<DBModel.Attribute>()
                                             where a.houseconfig_id.Equals(SelectedHouse.Id)
                                             && a.attribute_id.Equals(b.attribute_id)
                                             && b.attribute_group_id.Equals(6)
                                             select new ImageInherit()
                                             {
                                                 SourceImage = b.rootfolder,
                                                 Id = b.attribute_id,
                                                 Description = b.description,
                                                 Price = b.price
                                             }).FirstOrDefault(),
                                 RoofMaterial = (from a in con.Table<Houseconfig_Has_Attribute>()
                                                 from b in con.Table<DBModel.Attribute>()
                                                 where a.houseconfig_id.Equals(SelectedHouse.Id)
                                                 && a.attribute_id.Equals(b.attribute_id)
                                                 && b.attribute_group_id.Equals(61)
                                                 select new ImageInherit()
                                                 {
                                                     SourceImage = b.rootfolder,
                                                     Id = b.attribute_id,
                                                     Description = b.description,
                                                     Price = b.price
                                                 }).FirstOrDefault(),
                                 Window = (from a in con.Table<Houseconfig_Has_Attribute>()
                                           from b in con.Table<DBModel.Attribute>()
                                           where a.houseconfig_id.Equals(SelectedHouse.Id)
                                           && a.attribute_id.Equals(b.attribute_id)
                                           && b.attribute_group_id.Equals(7)
                                           select new ImageInherit()
                                           {
                                               SourceImage = b.rootfolder,
                                               Id = b.attribute_id,
                                               Description = b.description,
                                               Price = b.price
                                           }).FirstOrDefault(),
                                 WindowColor = (from a in con.Table<Houseconfig_Has_Attribute>()
                                                from b in con.Table<DBModel.Attribute>()
                                                where a.houseconfig_id.Equals(SelectedHouse.Id)
                                                && a.attribute_id.Equals(b.attribute_id)
                                                && b.attribute_group_id.Equals(903)
                                                select new ColorPalette
                                                (b.attribute_id, Byte.Parse(b.description.Split(',').GetValue(0).ToString()), Byte.Parse(b.description.Split(',').GetValue(1).ToString()), Byte.Parse(b.description.Split(',').GetValue(2).ToString()))).FirstOrDefault(),
                                 Door = (from a in con.Table<Houseconfig_Has_Attribute>()
                                         from b in con.Table<DBModel.Attribute>()
                                         where a.houseconfig_id.Equals(SelectedHouse.Id)
                                         && a.attribute_id.Equals(b.attribute_id)
                                         && b.attribute_group_id.Equals(71)
                                         select new ImageInherit()
                                         {
                                             SourceImage = b.rootfolder,
                                             Id = b.attribute_id,
                                             Description = b.description,
                                             Price = b.price
                                         }).FirstOrDefault(),
                                 DoorColor = (from a in con.Table<Houseconfig_Has_Attribute>()
                                              from b in con.Table<DBModel.Attribute>()
                                              where a.houseconfig_id.Equals(SelectedHouse.Id)
                                              && a.attribute_id.Equals(b.attribute_id)
                                              && b.attribute_group_id.Equals(904)
                                              select new ColorPalette
                                              (b.attribute_id, Byte.Parse(b.description.Split(',').GetValue(0).ToString()), Byte.Parse(b.description.Split(',').GetValue(1).ToString()), Byte.Parse(b.description.Split(',').GetValue(2).ToString()))).FirstOrDefault(),
                                 EnergySystem = (from a in con.Table<Houseconfig_Has_Attribute>()
                                                 from b in con.Table<DBModel.Attribute>()
                                                 where a.houseconfig_id.Equals(SelectedHouse.Id)
                                                 && a.attribute_id.Equals(b.attribute_id)
                                                 && b.attribute_group_id.Equals(8)
                                                 select new EHSystem()
                                                 {
                                                     Id = b.attribute_id,
                                                     Name = b.description,
                                                     Price = b.price
                                                 }).FirstOrDefault(),
                                 HeatingSystem = (from a in con.Table<Houseconfig_Has_Attribute>()
                                                  from b in con.Table<DBModel.Attribute>()
                                                  where a.houseconfig_id.Equals(SelectedHouse.Id)
                                                  && a.attribute_id.Equals(b.attribute_id)
                                                  && b.attribute_group_id.Equals(81)
                                                  select new EHSystem()
                                                  {
                                                      Id = b.attribute_id,
                                                      Name = b.description,
                                                      Price = b.price
                                                  }).FirstOrDefault(),
                                 NumberOfSocket = (from a in con.Table<Houseconfig_Has_Attribute>()
                                                   from b in con.Table<DBModel.Attribute>()
                                                   where a.houseconfig_id.Equals(SelectedHouse.Id)
                                                   && a.attribute_id.Equals(b.attribute_id)
                                                   && b.attribute_group_id.Equals(12)
                                                   select b.description).FirstOrDefault(),
                                 Chimney = (from a in con.Table<Houseconfig_Has_Attribute>()
                                            from b in con.Table<DBModel.Attribute>()
                                            where a.houseconfig_id.Equals(SelectedHouse.Id)
                                            && a.attribute_id.Equals(b.attribute_id)
                                            && b.attribute_group_id.Equals(9)
                                            select new ImageInherit()
                                            {
                                                SourceImage = b.rootfolder,
                                                Id = b.attribute_id,
                                                Description = b.description,
                                                Price = b.price
                                            }).FirstOrDefault(),
                                 Pool = (from a in con.Table<Houseconfig_Has_Attribute>()
                                         from b in con.Table<DBModel.Attribute>()
                                         where a.houseconfig_id.Equals(SelectedHouse.Id)
                                         && a.attribute_id.Equals(b.attribute_id)
                                         && b.attribute_group_id.Equals(10)
                                         select new ImageInherit()
                                         {
                                             SourceImage = b.rootfolder,
                                             Id = b.attribute_id,
                                             Description = b.description,
                                             Price = b.price
                                         }).FirstOrDefault(),
                                 Poolsize = (from a in con.Table<Houseconfig_Has_Attribute>()
                                             from b in con.Table<DBModel.Attribute>()
                                             where a.houseconfig_id.Equals(SelectedHouse.Id)
                                             && a.attribute_id.Equals(b.attribute_id)
                                             && b.attribute_group_id.Equals(11)
                                             select b.description).FirstOrDefault(),
                                 Fence = (from a in con.Table<Houseconfig_Has_Attribute>()
                                          from b in con.Table<DBModel.Attribute>()
                                          where a.houseconfig_id.Equals(SelectedHouse.Id)
                                          && a.attribute_id.Equals(b.attribute_id)
                                          && b.attribute_group_id.Equals(101)
                                          select new ImageInherit()
                                          {
                                              SourceImage = b.rootfolder,
                                              Id = b.attribute_id,
                                              Description = b.description,
                                              Price = b.price
                                          }).FirstOrDefault(),
                                 FenceColor = (from a in con.Table<Houseconfig_Has_Attribute>()
                                               from b in con.Table<DBModel.Attribute>()
                                               where a.houseconfig_id.Equals(SelectedHouse.Id)
                                               && a.attribute_id.Equals(b.attribute_id)
                                               && b.attribute_group_id.Equals(905)
                                               select new ColorPalette
                                               (b.attribute_id, Byte.Parse(b.description.Split(',').GetValue(0).ToString()), Byte.Parse(b.description.Split(',').GetValue(1).ToString()), Byte.Parse(b.description.Split(',').GetValue(2).ToString()))).FirstOrDefault(),
                                 Garage = (from a in con.Table<Houseconfig_Has_Attribute>()
                                           from b in con.Table<DBModel.Attribute>()
                                           where a.houseconfig_id.Equals(SelectedHouse.Id)
                                           && a.attribute_id.Equals(b.attribute_id)
                                           && b.attribute_group_id.Equals(13)
                                           select new EHSystem()
                                           {
                                               Id = b.attribute_id,
                                               Name = b.description,
                                               Price = b.price
                                           }).FirstOrDefault()
                             }).Single();
                    con.Close();
                }
            }
            catch (Exception)
            {

            }
            return house;
        }
        private List<HouseSummary> SQLGetHouseconfig()
        {
            List<HouseSummary> house = new List<HouseSummary>();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    house = (from a in con.Table<Mdh_Users>()
                             from b in con.Table<Houseconfig>()
                             from f in con.Table<Ymdh_House_Package>()
                             from c in con.Table<Ymdh_Address>()
                             where a.id.Equals(b.customer_user_id)
                             && a.id.Equals(selectedCustomerList.Id)
                             && b.house_package_id.Equals(f.house_package_id)
                             && b.status.Equals("1")
                             && f.address_id.Equals(c.mdh_address_id)
                             select new HouseSummary
                             {
                                 Id = b.houseconfig_id,
                                 Price = b.price,
                                 ConfDate = b.modifieddate,
                                 Customer = new Customer(a.id, a.name, SQLCustomerCountProject(a.id, con), SQLCustomerCountHouseconfig(a.id, con)),
                                 Consultant = new Consultant(b.consultant_user_id, SQLGetConsultantName(b.consultant_user_id, con)),
                                 Package = new ImageInherit(f.rootfolder, f.house_package_id, f.description, f.price, c.ZIP, c.City, c.Street, c.houseno, c.country)
                             }).ToList();

                    con.Close();
                }
            }
            catch (Exception)
            {

            }
            return house;
        }

        private List<ImageInherit> SQLGetGroundPlots(int Hid, SQLiteConnection con)
        {
            List<Housefloor> hfl;
            List<ImageInherit> gpl = new List<ImageInherit>();
            hfl = (from a in con.Table<Housefloor>()
                   where a.houseconfig_id.Equals(Hid)
                   select a).ToList();

            foreach (var item in hfl)
            {
                gpl.Add(new ImageInherit(item.rootfolder, item.housefloor_id, item.area.ToString(), item.price));
            }

            return gpl;
        }
        #endregion
        #endregion

        #region UseCase Customer
        public RelayCommand ButtonCreateNewCustomer { get; set; }
        public RelayCommand ButtonEditConfigurationCustomer { get; set; }
        private ObservableCollection<Customer> listCustomer  = new ObservableCollection<Customer>();

        public ObservableCollection<Customer> ListCustomer
        {
            get { return listCustomer; }
            set { listCustomer = value; }
        }

        private Customer selectedCustomerList;

        public Customer SelectedCustomerList
        {
            get { return selectedCustomerList; }
            set {
                selectedCustomerList = value;
                OnChange("SelectedCustomerList");
                CreatePdf();             
            }
        }

        private string pDFButtonVisibility = "Collapsed";

        public string PDFButtonVisibility
        {
            get { return pDFButtonVisibility; }
            set {
                if (SelectedConfHouse == null)
                {
                    pDFButtonVisibility = "Collapsed";
                }
                else
                {
                    pDFButtonVisibility = value;
                    OnChange("PDFButtonVisibility");
                }
            }
        }
        public void ButtonEditConfigurationCustomerMethod()
        {
            if(selectedConfHouse != null)
            {
                SelectedConfHouse = SQLGetHouseconfig2(SelectedConfHouse);
                TotalPrice = SelectedConfHouse.Price;
                //SelectedItems
                //HausKonfigurator
                SelectedCustomerr = selectedConfHouse.Customer;
                SelectedItemFloor = selectedConfHouse.numberOfFloors;
                SelectedHouse = selectedConfHouse.Package;
                SelectedPlot = selectedConfHouse.Plot;
                SelectedFloor = selectedConfHouse.GroundPlots[0];

                FloorsGroundPlot.Clear();
                foreach (var item in FloorsGroundPlot)
                {
                    FloorsGroundPlot.Add(item);
                }

                SelectedOutsideWall = selectedConfHouse.OutsideWall;
                SelectedColorOutsideWall = selectedConfHouse.OutsideWallColor;
                SelectedInsideWall = selectedConfHouse.InsideWall;
                SelectedColorInsideWall = selectedConfHouse.InsideWallColor;
                SelectedRoofType = selectedConfHouse.RoofType;
                SelectedRoofMaterial = selectedConfHouse.RoofMaterial;
                SelectedWindow = selectedConfHouse.Window;
                SelectedColorWindow = selectedConfHouse.WindowColor;
                SelectedDoor = selectedConfHouse.Door;
                SelectedColorDoor = selectedConfHouse.DoorColor;
                SelectedEnergySystem = selectedConfHouse.EnergySystem;
                SelectedHeatingSystem = selectedConfHouse.HeatingSystem;
                SelectedSocket = new ImageInherit(0, selectedConfHouse.NumberOfSocket.ToString(), 0);
                SelectedChimney = selectedConfHouse.Chimney;
                SelectedPoolSize = new ImageInherit(0, selectedConfHouse.Poolsize, 0);
                SelectedPool = selectedConfHouse.Pool;
                SelectedFence = selectedConfHouse.Fence;
                SelectedColorFence = selectedConfHouse.FenceColor;

                //TODO: Notitzen nicht vergessen

                ButtonForwardSummaryMethod();
            }
           
        }

        //Create new User
        public void ButtonCreateNewCustomerMethod()
        {
            GetFrame();
            a.Navigate(typeof(Pages.KundenPages.CreateCustomerPage));
        }

        #endregion

        #region GeneralFunctions
        /// <summary>
        /// In dieser Region werden allgemeine Funktionien gepseichert
        /// </summary>

        // Resetet alle Listen und selected Items
        public void ResetProperties()
        {
            //Variablen
            //HausKonfigurator
            House = new HouseSummary();
            TotalPrice = 0;
            NumberOfFloorDB = 0;
            //Appointments
            DateAppointment = new DateTime();
            TimeAppoitment = new TimeSpan();
            DateTimeNow = new DateTime();

            //Listen
            //HausKonfigurator
            Customers.Clear();
            ImagesHouse.Clear();
            imagesPlot.Clear();
            NumberFloors.Clear();
            FloorsGroundPlot.Clear();
            ListOutsideWall.Clear();
            ListColorOutsideWall.Clear();
            ListInsideWall.Clear();
            ListColorInsideWall.Clear();
            ListRoofType.Clear();
            ListRoofMaterial.Clear();
            ListWindows.Clear();
            ListColorWindows.Clear();
            ListDoors.Clear();
            ListColorDoors.Clear();
            ListEnergySystem.Clear();
            ListHeatingSystem.Clear();
            NumberSockets.Clear();
            ListChimneys.Clear();
            NumberPoolSizes.Clear();
            ListPools.Clear();
            ListFence.Clear();
            ListColorFence.Clear();
            //Appintments
            Appointments.Clear();
            Consultants.Clear();

            //SelectedItems
            //HausKonfigurator
            SelectedCustomerr = new Customer();
            SelectedItemFloor = 0;
            SelectedHouse = new ImageInherit();
            SelectedPlot = new ImageInherit();
            SelectedFloor = new ImageInherit();
            SelectedOutsideWall = new ImageInherit();
            SelectedColorOutsideWall = new ColorPalette();
            SelectedInsideWall = new ImageInherit();
            SelectedColorInsideWall = new ColorPalette();
            SelectedRoofType = new ImageInherit();
            SelectedRoofMaterial = new ImageInherit();
            SelectedWindow = new ImageInherit();
            SelectedColorWindow = new ColorPalette();
            SelectedDoor = new ImageInherit();
            SelectedColorDoor = new ColorPalette();
            SelectedEnergySystem = new EHSystem();
            SelectedHeatingSystem = new EHSystem();
            SelectedSocket = new ImageInherit();
            SelectedChimney = new ImageInherit();
            SelectedPoolSize = new ImageInherit();
            SelectedPool = new ImageInherit();
            SelectedFence = new ImageInherit();
            SelectedColorFence = new ColorPalette();
            CheckBoxIsSelected = false;
            //Appointments
            selectedAppointment = new Appointment();
            SelectedConsultant = new Consultant();

        }
        #endregion

        #region Test Methoden


        //MessageBox wird angezeigt -> zum Testen
        private async void AsynchMethod()
        {
            var dialog = new MessageDialog("TestMethode wurde aufgerufen");
            await dialog.ShowAsync();
        }

        //Command zum anclicken einer liste
        //http://stackoverflow.com/questions/18257516/how-to-pass-listbox-selecteditem-as-command-parameter-in-a-button
        /*private RelayCommand someComboBoxCommand;
        public RelayCommand SomeComboBoxCommand
        {
            get { return someComboBoxCommand; }
            set { someComboBoxCommand = value; }
        }
        */
        //Initialisierung
        //SomeComboBoxCommand = new RelayCommand(TestMethode);

        //Test Methode
        /*
        private async void TestMethode()
        {
            var dialog = new MessageDialog(selectedCustomerr.Firstname);
            await dialog.ShowAsync();
            GetFrame();
            a.Navigate(typeof(Pages.KundenPage));
        }
        */
        #endregion
    }
}
