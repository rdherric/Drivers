// Device.cpp : Implementation of CDevice

#include "stdafx.h"
#include "Device.h"

// IDevice
/*****************************************************
* Config is called to allow the Device to set itself
* up in the the Framework by registering callbacks.
*****************************************************/
HRESULT STDMETHODCALLTYPE CDevice::Init(IWDFDriver *FxWdfDriver, IWDFDeviceInitialize *FxDeviceInit)
{
    //Set no locking -- no automatic callback synchronization
    FxDeviceInit->SetLockingConstraint(None);

    //Create a new interfaces to send to the CreateDevice method
	CComPtr<IUnknown> unknown(static_cast<IDevice*>(this));
	CComPtr<IWDFDevice> fxDevice;

	//Create the device by sending the IUnknown callback 
	//object to handle any device-level events that occur
    HRESULT hr = FxWdfDriver->CreateDevice(FxDeviceInit, unknown, &fxDevice);

	//Return the result
    return hr;
}


//IPnpCallbackHardware
/*****************************************************
* OnPrepareHardware is called when an actual Device
* is dicovered on the bus.  Uses UMDF interfaces
* to create the Pipes and Queues on the Device.
*****************************************************/
HRESULT STDMETHODCALLTYPE CDevice::OnPrepareHardware(__in IWDFDevice *FxDevice)
{
	//Prepare the read and write USB Pipes
	HRESULT hr = this->RegisterUsbPipes(FxDevice);

	//If that failed, return the failure code
	if (FAILED(hr))
		return hr;

	//Prepare the read and write Queues
	hr = this->RegisterIOQueues(FxDevice);

	//Return the result
    return hr;
}


/*****************************************************
* OnReleaseHardware is called when the Device is
* removed or stopped.  Releases all of the resources
* that were allocated by OnPrepareHardware.
*****************************************************/
HRESULT STDMETHODCALLTYPE CDevice::OnReleaseHardware(__in IWDFDevice *FxDevice)
{
	//Show the compiler that we're not using the 
	//parameter -- got a copy already
	UNREFERENCED_PARAMETER(FxDevice);

	//Release the Pipes if they have been created
	if (this->_fxBulkInputPipe != NULL)
	{
		this->_fxBulkInputPipe->DeleteWdfObject();
		this->_fxBulkInputPipe = NULL;
	}

	if (this->_fxBulkOutputPipe != NULL)
	{
		this->_fxBulkOutputPipe->DeleteWdfObject();
		this->_fxBulkOutputPipe = NULL;
	}

	if (this->_fxControlPipe != NULL)
	{
		this->_fxControlPipe->DeleteWdfObject();
		this->_fxControlPipe = NULL;
	}

	//Return success
	return S_OK;
}


//IQueueCallbackRead 
/*****************************************************
* OnRead is called when an I/O Read Request comes
* through on the Queue.  
*****************************************************/
void STDMETHODCALLTYPE CDevice::OnRead(__in IWDFIoQueue *FxQueue, __in IWDFIoRequest *FxRequest, __in SIZE_T numBytesToRead)
{
	//Don't need the Queue reference or the number
	//of Bytes to read
    UNREFERENCED_PARAMETER(FxQueue);
	UNREFERENCED_PARAMETER(numBytesToRead);

	//Get the Memory from the Request
	CComPtr<IWDFMemory> memory;
	FxRequest->GetOutputMemory(&memory);

	//If the memory was retrieved properly, try 
	//to send it to the USB device
	HRESULT hr = S_OK;
	if (memory != NULL)
	{
		hr = this->_fxBulkInputPipe->FormatRequestForRead(
			FxRequest,			//Request Object
		    NULL,				//Pointer to File
			memory,				//Memory object
			NULL,				//Memory offset
		    NULL);				//DeviceOffset
	}

	//If anything bad happened, Complete the Request
	//with the error code.  Otherwise, send off the 
	//data to the USB Pipe.
    if (FAILED(hr) == TRUE)
        FxRequest->Complete(hr);
	else 
		this->SendRequest(FxRequest, this->_fxBulkInputPipe);
}


//IQueueCallbackWrite 
/*****************************************************
* OnWrite is called when an I/O Write Request comes
* through on the Queue.  
*****************************************************/
void STDMETHODCALLTYPE CDevice::OnWrite(__in IWDFIoQueue *FxQueue, __in IWDFIoRequest *FxRequest, __in SIZE_T numBytesToWrite)
{
	//Don't need the Queue reference or number of 
	//Bytes to write
    UNREFERENCED_PARAMETER(FxQueue);
	UNREFERENCED_PARAMETER(numBytesToWrite);

	//Get the Memory from the Request
	CComPtr<IWDFMemory> memory;
	FxRequest->GetInputMemory(&memory);

	//If the memory was retrieved properly, try 
	//to send it to the USB device
	HRESULT hr = S_OK;
	if (memory != NULL)
	{
		hr = this->_fxBulkOutputPipe->FormatRequestForWrite(
			FxRequest,			//Request Object
		    NULL,				//Pointer to File
			memory,				//Memory object
			NULL,				//Memory offset
		    NULL);				//DeviceOffset
	}

	//If anything bad happened, Complete the Request
	//with the error code.  Otherwise, send off the 
	//data to the USB Pipe.
    if (FAILED(hr) == TRUE)
        FxRequest->Complete(hr);
	else 
		this->SendRequest(FxRequest, this->_fxBulkOutputPipe);
}


//IQueueCallbackDeviceIoControl
/*****************************************************
* OnDeviceIoControl is called when an I/O Control 
* Request comes through on the Queue.  
*****************************************************/
void STDMETHODCALLTYPE CDevice::OnDeviceIoControl(__in IWDFIoQueue *FxQueue, __in IWDFIoRequest *FxRequest, __in ULONG ControlCode,
												  __in SIZE_T InputBufferSizeInBytes, __in SIZE_T OutputBufferSizeInBytes)
{
	//Don't need the reference to the Queue
	UNREFERENCED_PARAMETER(FxQueue);
	UNREFERENCED_PARAMETER(InputBufferSizeInBytes);
	UNREFERENCED_PARAMETER(OutputBufferSizeInBytes);

	//Get the Memory from the Request
	CComPtr<IWDFMemory> inputMemory;
	FxRequest->GetInputMemory(&inputMemory);

	CComPtr<IWDFMemory> outputMemory;
	FxRequest->GetOutputMemory(&outputMemory);

	//If any memory was retrieved properly, try 
	//to send it to the USB device
	HRESULT hr = S_OK;
	if (inputMemory != NULL || outputMemory != NULL)
	{
		hr = this->_fxControlPipe->FormatRequestForIoctl(
			FxRequest,			//Request Object
			ControlCode,		//I/O Control Code
			NULL,				//Pointer to File
			inputMemory,		//Input Memory Object
			0,					//Input Memory Offset
			outputMemory,		//Output Memory Object
			0);					//Output Memory Offset
	}

	//If anything bad happened, Complete the Request
	//with the error code.  Otherwise, send off the 
	//data to the USB Pipe.
    if (FAILED(hr) == TRUE)
        FxRequest->Complete(hr);
	else 
		this->SendRequest(FxRequest, this->_fxControlPipe);
}


//IRequestCallbackRequestCompletion 
/*****************************************************
* OnCompletion formats the Request to be returned
* to the application.
*****************************************************/
void STDMETHODCALLTYPE CDevice::OnCompletion(__in IWDFIoRequest* fxRequest, __in IWDFIoTarget* fxTarget,
										     __in IWDFRequestCompletionParams*  compParams, __in PVOID context)
{
	//Don't need the references to the target
	//or the context
    UNREFERENCED_PARAMETER(fxTarget);
    UNREFERENCED_PARAMETER(context);

	//Complete the Request
    fxRequest->CompleteWithInformation(
        compParams->GetCompletionStatus(),
        compParams->GetInformation());
}


//CDevice Private Methods
/*****************************************************
* RegisterIOQueues does the work of creating the 
* actual Framework Queues to send and retrieve 
* messages with an application.
*****************************************************/
HRESULT STDMETHODCALLTYPE CDevice::RegisterIOQueues(IWDFDevice* fxDevice)
{
	//Get an IUnknown interface from this
	CComPtr<IUnknown> unknown(static_cast<IDevice*>(this));

	//Declare a variable to return
	HRESULT hr = S_OK;

	//Create the Bulk Input I/O Queue for the Device
	//if an endpoint was found
	if (this->_fxBulkInputPipe != NULL)
	{
		CComPtr<IWDFIoQueue> inputQueue;
		hr = fxDevice->CreateIoQueue(
			unknown,						//IUnknown interface for callbacks
			FALSE,							//Not the Default Queue
			WdfIoQueueDispatchSequential,	//Sequential Dispatch type 
			TRUE,							//Power-managed Queue
			FALSE,							//No zero-length Requests
			&inputQueue);					//Pointer to Queue

		//If the Queue was created, set the type to Write
		if (SUCCEEDED(hr))
			hr = inputQueue->ConfigureRequestDispatching(WdfRequestWrite, TRUE);
	}

	//If that failed, just return
	if (FAILED(hr))
		return hr;

	//Create the Bulk Output I/O Queue for the Device
	//if an endpoint was found
	if (this->_fxBulkOutputPipe != NULL)
	{
		CComPtr<IWDFIoQueue> outputQueue;
		hr = fxDevice->CreateIoQueue(
			unknown,						//IUnknown interface for callbacks
			FALSE,							//Not the Default Queue
			WdfIoQueueDispatchSequential,	//Sequential Dispatch type 
			TRUE,							//Power-managed Queue
			FALSE,							//No zero-length Requests
			&outputQueue);					//Pointer to Queue

		//If the Queue was created, set the type to Read
		if (SUCCEEDED(hr))
			hr = outputQueue->ConfigureRequestDispatching(WdfRequestRead, TRUE);
	}

	//If that failed, just return
	if (FAILED(hr))
		return hr;

	//Create the Control I/O Queue for the Device
	//if an endpoint was found
	if (this->_fxControlPipe != NULL)
	{
		CComPtr<IWDFIoQueue> controlQueue;
		hr = fxDevice->CreateIoQueue(
			unknown,						//IUnknown interface for callbacks
			FALSE,							//Not the Default Queue
			WdfIoQueueDispatchSequential,	//Sequential Dispatch type 
			TRUE,							//Power-managed Queue
			FALSE,							//No zero-length Requests
			&controlQueue);					//Pointer to Queue

		//If the Queue was created, set the type to Control
		if (SUCCEEDED(hr))
			hr = controlQueue->ConfigureRequestDispatching(WdfRequestDeviceIoControl, TRUE);
	}

	//Return the result
	return hr;
}


/*****************************************************
* RegisterUsbPipes does the work of creating the 
* actual Framework USB Pipes to communicate with 
* the underlying USB device.
*****************************************************/
HRESULT STDMETHODCALLTYPE CDevice::RegisterUsbPipes(IWDFDevice* fxDevice)
{
	//Get a factory interface pointer
	CComQIPtr<IWDFUsbTargetFactory> factory(fxDevice);

	//If the pointer was QIed properly, get the 
	//Target Device interface
	CComPtr<IWDFUsbTargetDevice> target;
	HRESULT hr = S_OK;
	if (factory != NULL)
        hr = factory->CreateUsbTargetDevice(&target);

	//If the Target was retrieved properly, try to get
	//the first Interface on the first Configuration
	CComPtr<IWDFUsbInterface> iface;
    if (SUCCEEDED(hr) && target->GetNumInterfaces() > 0)
		hr = target->RetrieveUsbInterface(0, &iface);

	//If that succeeded, iterate through all of the pipes
	//and save them in the member variables
	if (SUCCEEDED(hr))
	{
		//Get the number of endpoints
		UCHAR numEndPoints = iface->GetNumEndPoints();

		//Iterate through and assign them all
		for (UCHAR i = 0; i < numEndPoints; i++)
		{
			//Get the current Pipe
			CComPtr<IWDFUsbTargetPipe> curPipe;
			hr = iface->RetrieveUsbPipeObject(i, &curPipe);

			//If that succeeded, assign the Endpoint to 
			//the member variable
			if (SUCCEEDED(hr))
			{
				if (curPipe->IsInEndPoint() && curPipe->GetType() == UsbdPipeTypeBulk)
					this->_fxBulkInputPipe = curPipe;
				else if (curPipe->IsOutEndPoint() && curPipe->GetType() == UsbdPipeTypeBulk)
					this->_fxBulkOutputPipe = curPipe;
				else if (curPipe->GetType() == UsbdPipeTypeControl)
					this->_fxControlPipe = curPipe;
				else
					curPipe->DeleteWdfObject();
			}
		}
	}

	//Return the result
	return hr;
}


/*****************************************************
* SendRequest does the actual sending of the IoRequest
* to the USB Device.
*****************************************************/
void STDMETHODCALLTYPE CDevice::SendRequest(IWDFIoRequest* fxRequest, IWDFIoTarget* target)
{
	//Get a Request Completion callback from this
    CComPtr<IRequestCallbackRequestCompletion> rxCallback(this);

	//Setup the callback for the Request
	fxRequest->SetCompletionCallback(rxCallback, NULL);

	//Send the request to the Target
    HRESULT hr = fxRequest->Send(
		target,									//The target to get the Request
		WDF_REQUEST_SEND_OPTION_SYNCHRONOUS,	//Flags for the Send
		0);										//Request send timeout

	//If that failed, just complete the Request
    if (FAILED(hr))
        fxRequest->CompleteWithInformation(hr, 0);
}