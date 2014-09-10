// Driver.cpp : Implementation of CDriver

#include "stdafx.h"
#include "Driver.h"

//IDriverEntry methods
/*****************************************************
* OnInitialize is called to set up the Driver
* when it is first loaded.  Doesn't do anything
* because all initialization is done in the 
* DeviceAdd event.
*****************************************************/
HRESULT STDMETHODCALLTYPE CDriver::OnInitialize(__in IWDFDriver *FxWdfDriver)
{
	UNREFERENCED_PARAMETER(FxWdfDriver);
	return S_OK;
}


/*****************************************************
* OnDeviceAdd is called when the driver is first 
* initialized with a Device.
*****************************************************/
HRESULT STDMETHODCALLTYPE CDriver::OnDeviceAdd(__in IWDFDriver *FxWdfDriver, __in IWDFDeviceInitialize *FxDeviceInit)
{
	//Create the Device object and let it configure itself
	CComPtr<IDevice> device;
	HRESULT hr = device.CoCreateInstance(__uuidof(IDevice));

	//If that succeeded, do the configuration
	if (SUCCEEDED(hr) == TRUE)
		hr = device->Init(FxWdfDriver, FxDeviceInit);

	//Return the result
	return hr;
}


/*****************************************************
* OnDeinitialize is called to set up the Driver
* when it is about to be unloaded.  Doesn't do 
* anything, either.
*****************************************************/
VOID STDMETHODCALLTYPE CDriver::OnDeinitialize(__in IWDFDriver *FxWdfDriver)
{
	UNREFERENCED_PARAMETER(FxWdfDriver);
}
