// Driver.h : Declaration of the CDriver

#pragma once
#include "resource.h"       // main symbols


#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif


// CDriver
class ATL_NO_VTABLE CDriver :
	public CComObjectRootEx<CComSingleThreadModel>,
	public IDriverEntry
{
public:
	CDriver()
	{
	}

DECLARE_NOT_AGGREGATABLE(CDriver)

BEGIN_COM_MAP(CDriver)
	COM_INTERFACE_ENTRY(IDriverEntry)
END_COM_MAP()

//IDriverEntry Methods
public:
	virtual HRESULT STDMETHODCALLTYPE OnInitialize(__in IWDFDriver *FxWdfDriver);
    virtual HRESULT STDMETHODCALLTYPE OnDeviceAdd(__in IWDFDriver *FxWdfDriver, __in IWDFDeviceInitialize *FxDeviceInit);
    virtual VOID STDMETHODCALLTYPE OnDeinitialize(__in IWDFDriver *FxWdfDriver);


	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}
};
