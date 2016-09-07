﻿using System.Windows;

namespace ConcurrencySpike {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            DataContext = new MainWindowViewModel();
            InitializeComponent();
        }
    }
}
