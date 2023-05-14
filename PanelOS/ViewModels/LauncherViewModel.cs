using System;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PanelOS.Models;
using PanelOS.Views;
using PanelOS.GameInteraction;
using MaterialDesignThemes.Wpf;
using PanelOS.Properties;
using System.Windows;
using PanelOS.Helpers;
using System.Threading.Tasks;
using System.Threading;

namespace PanelOS.ViewModels
{
    class LauncherViewModel : INotifyPropertyChanged
    {
        public LauncherDbContext DbContext { get; set; }

        private IEnumerable<Account> accounts;
        private IEnumerable<Account> filteredAccounts;
        private IEnumerable<CalibrationLobby> calibrationLobbies;
        private IEnumerable<BoostingLobby> boostingLobbies;

        private RelayCommand addAccountCommand;
        private RelayCommand deleteAccountCommand;

        private RelayCommand startAccountsCommand;
        private RelayCommand restartAccountsCommand;
        private RelayCommand stopAccountsCommand;
        private RelayCommand stopAllAccountsCommand;

        private RelayCommand addCalibrationLobbyCommand;
        private RelayCommand deleteCalibrationLobbyCommand;
        private RelayCommand startCalibrationLobbyCommand;

        private RelayCommand addBoostingLobbyCommand;
        private RelayCommand deleteBoostingLobbyCommand;
        private RelayCommand startBoostingLobbyCommand;

        private RelayCommand accountColorChangedCommand;
        private RelayCommand selectedColorChangedCommand;
        
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        private ColorFilter[] colorFilters;
        private bool SingleAccDisposed;

        public SnackbarMessageQueue LauncherMessageQueue { get; set; }

        public IEnumerable<Account> Accounts
        {
            get { return accounts; }
            set
            {
                accounts = value;
                OnPropertyChanged("Accounts");
            }
        }

        public IEnumerable<Account> FilteredAccounts
        {
            get { return filteredAccounts; }
            set
            {
                filteredAccounts = value;
                OnPropertyChanged("FilteredAccounts");
            }
        }

        public ColorFilter[] ColorFilters
        {
            get { return colorFilters; }
            set
            {
                colorFilters = value;
                OnPropertyChanged("ColorFilters");
            }
        }

        public IEnumerable<CalibrationLobby> CalibrationLobbies
        {
            get { return calibrationLobbies; }
            set
            {
                calibrationLobbies = value;
                OnPropertyChanged("CalibrationLobbies");
            }
        }

        public IEnumerable<BoostingLobby> BoostingLobbies
        {
            get { return boostingLobbies; }
            set
            {
                boostingLobbies = value;
                OnPropertyChanged("BoostingLobbies");
            }
        }

        public LauncherViewModel()
        {
            DbContext = new LauncherDbContext();

            ColorFilters = new ColorFilter[9]
            {
                new ColorFilter(){ BookmarkKind="BookmarkBorder", BookmarkColor = "Gray", Selected = false },
                new ColorFilter(){ BookmarkKind="Bookmark", BookmarkColor = "#DD2C00", Selected = false },
                new ColorFilter(){ BookmarkKind="Bookmark", BookmarkColor = "#E65100", Selected = false },
                new ColorFilter(){ BookmarkKind="Bookmark", BookmarkColor = "#F4B400", Selected = false },
                new ColorFilter(){ BookmarkKind="Bookmark", BookmarkColor = "Yellow", Selected = false },
                new ColorFilter(){ BookmarkKind="Bookmark", BookmarkColor = "#FF54D354", Selected = false },
                new ColorFilter(){ BookmarkKind="Bookmark", BookmarkColor = "#FF0B9552", Selected = false },
                new ColorFilter(){ BookmarkKind="Bookmark", BookmarkColor = "#29B6F6", Selected = false },
                new ColorFilter(){ BookmarkKind="Bookmark", BookmarkColor = "#4285F4", Selected = false }
            };

            DbContext.Accounts.Load();
            Accounts = DbContext.Accounts.Local.ToBindingList();
            FilteredAccounts = DbContext.Accounts.Local.ToBindingList();

            DbContext.BoostingLobbies.Load();
            BoostingLobbies = DbContext.BoostingLobbies.Local.ToBindingList();

            DbContext.CalibrationLobbies.Load();
            CalibrationLobbies = DbContext.CalibrationLobbies.Local.ToBindingList();

            LauncherMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(3000));

            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
        }

        #region Accounts

        public RelayCommand AddAccountCommand
        {
            get
            {
                return addAccountCommand ??
                  (addAccountCommand = new RelayCommand((o) =>
                  {
                      AddSteamAccountView addSteamAccountView = new AddSteamAccountView(new Account(), LauncherMessageQueue);

                      if (addSteamAccountView.ShowDialog() == true)
                      {
                          Account account = addSteamAccountView.Account;

                          if (DbContext.Accounts.Any(acc => acc.Login == account.Login))
                          {
                              LauncherMessageQueue.Enqueue("This account has already existed");
                              return;
                          }

                          bool accountFound = false;
                          bool tryingToAddAccount = true;                          

                          while (tryingToAddAccount)
                          {
                              dynamic loginusers = SteamCommands.GetLoginusers();

                              if (loginusers != null)
                              {
                                  foreach (dynamic loginUser in loginusers)
                                  {
                                      if (loginUser.Value["AccountName"].Value == account.Login.ToLower())
                                      {
                                          account.SteamProfileId = loginUser.Key;
                                          account.SteamUserId = (long.Parse(account.SteamProfileId) - 76561197960265728).ToString();

                                          tryingToAddAccount = false;
                                          accountFound = true;

                                          FileSystemCommands.CopySteamExecutable(account.SteamUserId);

                                          FileSystemCommands.CopyCsGoDataFolder("data/game/",
                                              Settings.Default.CsGoFolder + "\\csgo_" + account.SteamUserId);

                                          FileSystemCommands.WriteCsGoWindowTitle(account.SteamUserId,
                                              "	game	\"@ ACCOUNT: " + account.SteamUserId + " | LOGIN: " + account.Login + " | BOT\"");

                                          FileSystemCommands.CreateCsGoSubfolders(account.SteamUserId);

                                          DbContext.Accounts.Add(account);
                                          DbContext.SaveChanges();
                                          break;
                                      }
                                  }
                              }

                              if (!accountFound)
                              {
                                  SteamAccNotFoundView steamAccountNotFoundView = new SteamAccNotFoundView();
                                  SteamCommands.StartSteam(account.Password);

                                  if (steamAccountNotFoundView.ShowDialog() == true)
                                      tryingToAddAccount = true;
                                  else
                                      tryingToAddAccount = false;
                              }   
                          }
                      }
                  }));
            }
        }

        public RelayCommand DeleteAccountCommand
        {
            get
            {
                return deleteAccountCommand ??
                  (deleteAccountCommand = new RelayCommand((selectedItem) =>
                  {
                      if(Accounts.Where(acc => acc.IsChecked).Any(acc => acc.Active))
                      {
                          LauncherMessageQueue.Enqueue("Stop active accounts before deleting");
                          return;
                      }

                      foreach (Account account in Accounts.Where(acc => acc.IsChecked))
                      {
                          FileSystemCommands.DeleteSteamExecutable(account.SteamUserId);
                          FileSystemCommands.DeleteCsGoSubfolders(account.SteamUserId);
                      }

                      DbContext.Accounts.RemoveRange(Accounts.Where(acc => acc.IsChecked));
                      DbContext.SaveChanges();
                  }));
            }
        }

        public RelayCommand StartAccountsCommand
        {
            get
            {
                return startAccountsCommand ??
                  (startAccountsCommand = new RelayCommand((selectedItem) =>
                  {
                      if (calibrationLobbies.Any(cl => cl.Active) || boostingLobbies.Any(bl => bl.Active))
                      {
                          LauncherMessageQueue.Enqueue("Stop active lobbies first");
                          return;
                      }
                      else
                      {
                          Account[] accountsToStart = Accounts.Where(acc => acc.IsChecked && !acc.Active).ToArray();

                          foreach (Account account in accountsToStart)
                          {
                              account.IsChecked = false;
                              account.Active = true;
                          }

                          cancellationTokenSource = new CancellationTokenSource();
                          cancellationToken = cancellationTokenSource.Token;
                          SingleAccDisposed = false;

                          int x = 0, y = 0;
                          int xStep = Settings.Default.CsGoWindowX + 25;
                          int yStep = Settings.Default.CsGoWindowY + 50;

                          Task startAccountsTask = new Task(async () =>
                          {
                              foreach (Account account in accountsToStart)
                              {
                                  if (cancellationToken.IsCancellationRequested)
                                      break;

                                  FileSystemCommands.WriteCsGoWindowTitle(account.SteamUserId, 
                                      "	game	\"@ ACCOUNT: " + account.SteamUserId + " | LOGIN: " + account.Login + " | BOT\"");

                                  ServerCommands.StartAccount(account, x, y, "cfg_global");
                                  await Task.Delay(1000);

                                  x += xStep;

                                  if (x + xStep > SystemParameters.PrimaryScreenWidth)
                                  {
                                      x = 0;
                                      y += yStep;
                                  }

                                  if (y + yStep > SystemParameters.PrimaryScreenHeight)
                                      y = 0;
                              }
                          }, cancellationToken);

                          startAccountsTask.Start();
                      }
                  }));
            }
        }

        public RelayCommand RestartAccountsCommand
        {
            get
            {
                return restartAccountsCommand ??
                  (restartAccountsCommand = new RelayCommand(async (selectedItem) =>
                  {
                      Account[] accountsToRestart = Accounts.Where(acc => acc.IsChecked && acc.Active).ToArray();

                      foreach (Account account in accountsToRestart)
                      {
                          account.IsChecked = false;
                          account.Active = true;
                      }

                      if (Accounts.Any(acc => acc.Active))
                      {
                          cancellationTokenSource.Cancel();
                          cancellationTokenSource.Dispose();
                          await Task.Delay(2000);
                      }

                      cancellationTokenSource = new CancellationTokenSource();
                      cancellationToken = cancellationTokenSource.Token;

                      Task restartAccountsTask = new Task(async () =>
                      {
                          foreach (Account account in accountsToRestart)
                          {
                              if (cancellationToken.IsCancellationRequested)
                                  break;

                              ServerCommands.RestartAccount(account.Login);
                              await Task.Delay(1000);
                          }
                      }, cancellationToken);

                      restartAccountsTask.Start();
                  }));
            }
        }

        public RelayCommand StopAccountsCommand
        {
            get
            {
                return stopAccountsCommand ??
                  (stopAccountsCommand = new RelayCommand(async (selectedItem) =>
                  {
                      if (calibrationLobbies.Any(cl => cl.Active) || boostingLobbies.Any(bl => bl.Active))
                      {
                          LauncherMessageQueue.Enqueue("Stop active lobbies first");
                          return;
                      }
                      else
                      {
                          Account[] accountsToStop = Accounts.Where(acc => acc.IsChecked && acc.Active).ToArray();

                          foreach (Account account in accountsToStop)
                          {
                              account.IsChecked = false;
                              account.Active = false;
                          }

                          if (accountsToStop.Any(acc => acc.Active) && !SingleAccDisposed)
                          {
                              cancellationTokenSource.Cancel();
                              cancellationTokenSource.Dispose();
                              SingleAccDisposed = true;
                              await Task.Delay(2500);
                          }                         

                          foreach (Account account in accountsToStop)                           
                              ServerCommands.StopAccount(account.Login);
                      }
                  }));
            }
        }

        public RelayCommand StopAllAccountsCommand
        {
            get
            {
                return stopAllAccountsCommand ??
                  (stopAllAccountsCommand = new RelayCommand(async (selectedItem) =>
                  {
                      if (calibrationLobbies.Any(cl => cl.Active) || boostingLobbies.Any(bl => bl.Active))
                      {
                          LauncherMessageQueue.Enqueue("Stop active lobbies first");
                          return;
                      }
                      else
                      {
                          Account[] accountsToStop = Accounts.ToArray();

                          foreach (Account account in Accounts)
                          {
                              account.IsChecked = false;
                              account.Active = false;
                          }

                          if (accountsToStop.Any(acc => acc.Active) && !SingleAccDisposed)
                          {
                              cancellationTokenSource.Cancel();
                              cancellationTokenSource.Dispose();
                              await Task.Delay(2500);
                          }                          

                          ServerCommands.StopAllAccounts();                          
                      }
                  }));
            }
        }

        #endregion

        #region Calibration

        public RelayCommand AddCalibrationLobbyCommand
        {
            get
            {
                return addCalibrationLobbyCommand ??
                  (addCalibrationLobbyCommand = new RelayCommand((o) =>
                  {
                      AddCalibrationLobbyView addCalibrationLobbyView = new AddCalibrationLobbyView(new CalibrationLobby(), LauncherMessageQueue);

                      if (addCalibrationLobbyView.ShowDialog() == true)
                      {
                          CalibrationLobby calibrationLobby = addCalibrationLobbyView.CalibrationLobby;
                          DbContext.CalibrationLobbies.Add(calibrationLobby);
                          DbContext.SaveChanges();
                      }
                  }));
            }
        }        

        public RelayCommand DeleteCalibrationLobbyCommand
        {
            get
            {
                return deleteCalibrationLobbyCommand ??
                  (deleteCalibrationLobbyCommand = new RelayCommand((selectedItem) =>
                  {
                      CalibrationLobby calibrationLobby = selectedItem as CalibrationLobby;

                      if (calibrationLobby.Active)
                      {
                          LauncherMessageQueue.Enqueue("Stop lobby first");
                          return;
                      }

                      DbContext.CalibrationLobbies.Remove(calibrationLobby);
                      DbContext.SaveChanges();
                  }));
            }
        }

        public RelayCommand StartCalibrationLobbyCommand
        {
            get
            {
                return startCalibrationLobbyCommand ??
                  (startCalibrationLobbyCommand = new RelayCommand(async (selectedItem) =>
                  {
                      CalibrationLobby calibrationLobby = selectedItem as CalibrationLobby;                      

                      if (!calibrationLobby.Active)
                      {
                          if (Accounts.Any(account => account.Active))
                          {
                              LauncherMessageQueue.Enqueue("Stop other accounts or lobbies first");
                              return;
                          }

                          if (calibrationLobby.Players.Any(p => p == null))
                          {
                              LauncherMessageQueue.Enqueue("You have to select all 10 players");
                              return;
                          }                          

                          calibrationLobby.Active = true;

                          foreach (int playerId in calibrationLobby.Players)
                          {
                              DbContext.Accounts.Find(playerId).IsChecked = false;
                              DbContext.Accounts.Find(playerId).Active = true;
                          }

                          cancellationTokenSource = new CancellationTokenSource();
                          cancellationToken = cancellationTokenSource.Token;

                          int x = 0;

                          Task startCalibrationAccountsTask = new Task(async () =>
                          {
                              int leaderIndex = 1;

                              // Leaders
                              foreach (int playerId in calibrationLobby.Players.Take(2))
                              {
                                  if (cancellationToken.IsCancellationRequested)
                                      break;

                                  Account lobbyPlayer = DbContext.Accounts.Find(playerId);

                                  if (lobbyPlayer != null)
                                  {
                                      if (leaderIndex == 1)
                                      {
                                          FileSystemCommands.WriteCsGoWindowTitle(lobbyPlayer.SteamUserId,
                                              "	game	\"@ ACCOUNT: " + lobbyPlayer.SteamUserId + " | LOGIN: " + lobbyPlayer.Login + " | LEADER #1\"");

                                          ServerCommands.StartAccount(lobbyPlayer, x, 350, "cfg_leader1");
                                      }
                                      else if (leaderIndex == 2)
                                      {
                                          FileSystemCommands.WriteCsGoWindowTitle(lobbyPlayer.SteamUserId,
                                              "	game	\"@ ACCOUNT: " + lobbyPlayer.SteamUserId + " | LOGIN: " + lobbyPlayer.Login + " | LEADER #2\"");

                                          ServerCommands.StartAccount(lobbyPlayer, x, 350, "cfg_leader2");
                                      }

                                      await Task.Delay(1000);
                                      leaderIndex++;
                                      x += 850;
                                  }
                              }

                              await Task.Delay(1000);
                              x = 0;

                              // Winners
                              foreach (int playerId in calibrationLobby.Players.Skip(2).Take(4))
                              {
                                  if (cancellationToken.IsCancellationRequested)
                                      break;

                                  Account lobbyPlayer = DbContext.Accounts.Find(playerId);

                                  if (lobbyPlayer != null)
                                  {
                                      FileSystemCommands.WriteCsGoWindowTitle(lobbyPlayer.SteamUserId, 
                                          "	game	\"@ ACCOUNT: " + lobbyPlayer.SteamUserId + " | LOGIN: " + lobbyPlayer.Login + " | BOT\"");

                                      ServerCommands.StartAccount(lobbyPlayer, x, 0, "cfg_boost");
                                      await Task.Delay(1000);
                                      x += 425;
                                  }
                              }

                              await Task.Delay(1000);
                              x = 0;

                              // Losers
                              foreach (int playerId in calibrationLobby.Players.Skip(6))
                              {
                                  if (cancellationToken.IsCancellationRequested)
                                      break;

                                  Account lobbyPlayer = DbContext.Accounts.Find(playerId);

                                  if (lobbyPlayer != null)
                                  {
                                      FileSystemCommands.WriteCsGoWindowTitle(lobbyPlayer.SteamUserId,
                                          "	game	\"@ ACCOUNT: " + lobbyPlayer.SteamUserId + " | LOGIN: " + lobbyPlayer.Login + " | BOT\"");

                                      ServerCommands.StartAccount(lobbyPlayer, x, 700, "cfg_boost");
                                      await Task.Delay(1000);
                                      x += 425;
                                  }
                              }
                          }, cancellationToken);

                          startCalibrationAccountsTask.Start();
                      }
                      else
                      {
                          calibrationLobby.Active = false;

                          if (Accounts.Any(acc => acc.Active))
                          {
                              foreach (int playerId in calibrationLobby.Players)
                              {
                                  Account lobbyPlayer = DbContext.Accounts.Find(playerId);
                                  lobbyPlayer.Active = false;
                              }

                              cancellationTokenSource.Cancel();
                              cancellationTokenSource.Dispose();
                              await Task.Delay(2500);
                          }                          

                          foreach (int playerId in calibrationLobby.Players)
                          {
                              Account lobbyPlayer = DbContext.Accounts.Find(playerId);

                              if (lobbyPlayer != null)
                                  ServerCommands.StopAccount(lobbyPlayer.Login);                                 
                          }
                      }

                      DbContext.SaveChanges();
                  }));
            }
        }

        #endregion

        #region Boosting

        public RelayCommand AddBoostingLobbyCommand
        {
            get
            {
                return addBoostingLobbyCommand ??
                  (addBoostingLobbyCommand = new RelayCommand((o) =>
                  {
                      AddBoostingLobbyView addBoostingLobbyView = new AddBoostingLobbyView(new BoostingLobby(), LauncherMessageQueue);

                      if (addBoostingLobbyView.ShowDialog() == true)
                      {
                          BoostingLobby boostingLobby = addBoostingLobbyView.BoostingLobby;
                          DbContext.BoostingLobbies.Add(boostingLobby);
                          DbContext.SaveChanges();
                      }
                  }));
            }
        }

        public RelayCommand StartBoostingLobbyCommand
        {
            get
            {
                return startBoostingLobbyCommand ??
                  (startBoostingLobbyCommand = new RelayCommand(async (selectedItem) =>
                  {
                      BoostingLobby boostingLobby = selectedItem as BoostingLobby;

                      if (!boostingLobby.Active)
                      {
                          if(Accounts.Any(account => account.Active))
                          {
                              LauncherMessageQueue.Enqueue("Stop other accounts or lobbies first");
                              return;
                          }

                          if (boostingLobby.Players.Any(p => p == null))
                          {
                              LauncherMessageQueue.Enqueue("You have to select all 9 players");
                              return;
                          }

                          if (boostingLobby.Players.Distinct().Count() != 9)
                          {
                              LauncherMessageQueue.Enqueue("No duplicated accounts allowed");
                              return;
                          }                          

                          boostingLobby.Active = true;                          

                          foreach (int playerId in boostingLobby.Players)
                          {
                              DbContext.Accounts.Find(playerId).IsChecked = false;
                              DbContext.Accounts.Find(playerId).Active = true;
                          }

                          cancellationTokenSource = new CancellationTokenSource();
                          cancellationToken = cancellationTokenSource.Token;

                          int x = 0, y = 0;

                          Task startBoostingAccountsTask = new Task(async() =>
                          {
                              int playerIndex = 0;

                              foreach (int playerId in boostingLobby.Players)
                              {
                                  if (cancellationToken.IsCancellationRequested)
                                      return;

                                  Account lobbyPlayer = DbContext.Accounts.Find(playerId);

                                  if (lobbyPlayer != null)
                                  {
                                      FileSystemCommands.WriteCsGoWindowTitle(lobbyPlayer.SteamUserId,
                                          "	game	\"@ ACCOUNT: " + lobbyPlayer.SteamUserId + " | LOGIN: " + lobbyPlayer.Login + " | BOT\"");

                                      ServerCommands.StartAccount(lobbyPlayer, x, y, "cfg_boost");
                                      await Task.Delay(1000);

                                      x += 425;

                                      if (x == 1700)
                                      {
                                          x = 0;
                                          y += 350;
                                      }

                                      if (y == 1050)
                                          y = 0;
                                  }

                                  playerIndex++;
                              }
                          }, cancellationToken);

                          startBoostingAccountsTask.Start();
                      }
                      else
                      {
                          boostingLobby.Active = false;

                          if (Accounts.Any(acc => acc.Active))
                          {
                              foreach (int playerId in boostingLobby.Players)
                              {
                                  Account lobbyPlayer = DbContext.Accounts.Find(playerId);
                                  lobbyPlayer.Active = false;
                              }

                              cancellationTokenSource.Cancel();
                              cancellationTokenSource.Dispose();
                              await Task.Delay(2500);
                          }

                          foreach (int playerId in boostingLobby.Players)
                          {
                              Account lobbyPlayer = DbContext.Accounts.Find(playerId);

                              if (lobbyPlayer != null)
                                  ServerCommands.StopAccount(lobbyPlayer.Login);
                          }
                      }

                      DbContext.SaveChanges();
                  }));
            }
        }

        public RelayCommand DeleteBoostingLobbyCommand
        {
            get
            {
                return deleteBoostingLobbyCommand ??
                  (deleteBoostingLobbyCommand = new RelayCommand((selectedItem) =>
                  {
                      BoostingLobby boostingLobby = selectedItem as BoostingLobby;

                      if (boostingLobby.Active)
                      {
                          LauncherMessageQueue.Enqueue("Stop lobby first");
                          return;
                      }

                      DbContext.BoostingLobbies.Remove(boostingLobby);
                      DbContext.SaveChanges();
                  }));
            }
        }

        #endregion

        #region Colored bookmarks

        public RelayCommand AccountColorChangedCommand
        {
            get
            {
                return accountColorChangedCommand ??
                  (accountColorChangedCommand = new RelayCommand((selectedItem) =>
                  {
                      Tuple<object, object> parameters = (Tuple<object, object>)selectedItem;
                      ((Account)parameters.Item1).Color = parameters.Item2.ToString();                      

                      DbContext.SaveChanges();
                      SelectedColorChangedCommand.Execute(null);
                  }));                
            }
        }

        public RelayCommand SelectedColorChangedCommand
        {
            get
            {
                return selectedColorChangedCommand ??
                  (selectedColorChangedCommand = new RelayCommand((selectedItem) =>
                  {
                      if (ColorFilters.Where(c => c.Selected).Any(cf => cf.BookmarkColor == "Gray"))
                          FilteredAccounts = DbContext.Accounts.Local;
                      else
                          FilteredAccounts = DbContext.Accounts.Local.Where(
                              acc => ColorFilters.Where(c => c.Selected).Any(cf => acc.Color == cf.BookmarkColor));
                  }));
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}