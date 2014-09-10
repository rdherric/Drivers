// Device.h : Declaration of the CDevice

#pragma once
#include "resource.h"       // main symbols


#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif


// CDevice
class ATL_NO_VTABLE CDevice :
	public CComObjectRootEx<CComSingleThreadModel>,
	public IDevice,
	public IPnpCallbackHardware,
	public IQueueCallbackRead,
	public IQueueCallbackWrite,
	public IQueueCallbackDeviceIoControl,
	public IRequestCallbackRequestCompletion
{
private:
	CComPtr<IWDFUsbTargetPipe> _fxBulkInputPipe;
	CComPtr<IWDFUsbTargetPipe> _fxBulkOutputPipe;
	CComPtr<IWDFUsbTargetPipe> _fxControlPipe;

	//CDevice Private methods
	HRESULT STDMETHODCALLTYPE RegisterIOQueues(IWDFDevice* fxDevice);
	HRESULT STDMETHODCALLTYPE RegisterUsbPipes(IWDFDevice* fxDevice);
	void STDMETHODCALLTYPE SendRequest(IWDFIoRequest* fxRequest, IWDFIoTarget* target);

public:
	CDevice()
	{
	}

DECLARE_NOT_AGGREGATABLE(CDevice)

BEGIN_COM_MAP(CDevice)
	COM_INTERFACE_ENTRY(IDevice)
	COM_INTERFACE_ENTRY(IPnpCallbackHardware)
END_COM_MAP()

public:
//IDevice Methods
	virtual HRESULT STDMETHODCALLTYPE Init(__in IWDFDriver *FxWdfDriver, __in IWDFDeviceInitialize *FxDeviceInit);

//IPnpCallbackHardware Methods
    virtual HRESULT STDMETHODCALLTYPE OnPrepareHardware(__in IWDFDevice *FxDevice);
    virtual HRESULT STDMETHODCALLTYPE OnReleaseHardware(__in IWDFDevice *FxDevice);

//IQueueCallbackRead Methods
    virtual void STDMETHODCALLTYPE OnRead(__in IWDFIoQueue *FxQueue, __in IWDFIoRequest *FxRequest, __in SIZE_T numBytesToRead);

//IQueueCallbackWrite Methods
    virtual void STDMETHODCALLTYPE OnWrite(__in IWDFIoQueue *FxQueue, __in IWDFIoRequest *FxRequest, __in SIZE_T numBytesToWrite);

//IQueueCallbackDeviceIoControl Methods
	virtual void STDMETHODCALLTYPE OnDeviceIoControl(__in IWDFIoQueue *FxQueue, __in IWDFIoRequest *FxRequest, __in ULONG ControlCode,
													 __in SIZE_T InputBufferSizeInBytes, __in SIZE_T OutputBufferSizeInBytes);


//IRequestCallbackRequestCompletion Methods
	virtual void STDMETHODCALLTYPE OnCompletion(__in IWDFIoRequest* fxRequest, __in IWDFIoTarget* fxTarget,
										        __in IWDFRequestCompletionParams*  compParams, __in PVOID context);


	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}
};
