//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Core;
using Windows.Devices.Radios;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace RadioManagerSample
{
    public class RadioModel : INotifyPropertyChanged
    {
        private readonly Radio _radio;
        private bool _isEnabled;
        private readonly UIElement _parent;

        public RadioModel(Radio radio, UIElement parent)
        {
            _radio = radio;
            // Controlling the mobile broadband radio requires a restricted capability.
            _isEnabled = (radio.Kind != RadioKind.MobileBroadband);
            _parent = parent;
            _radio.StateChanged += Radio_StateChanged;
        }

        private async void Radio_StateChanged(Radio sender, object args)
        {
            // The Radio StateChanged event doesn't run from the UI thread, so we must use the dispatcher
            // to run NotifyPropertyChanged
            await _parent.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NotifyPropertyChanged("IsRadioOn");
            });
        }

        public string Name => _radio.Name;

        public bool IsRadioOn
        {
            get => _radio.State == RadioState.On;
            set => SetRadioState(value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void SetRadioState(bool isRadioOn)
        {
            var radioState = isRadioOn ? RadioState.On : RadioState.Off;
            Disable();
            await _radio.SetStateAsync(radioState);
            NotifyPropertyChanged("IsRadioOn");
            Enable();
        }

        private void Enable()
        {
            IsEnabled = true;
        }

        private void Disable()
        {
            IsEnabled = false;
        }
    }
}
