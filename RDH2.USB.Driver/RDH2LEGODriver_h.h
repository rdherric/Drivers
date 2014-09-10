

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0500 */
/* at Sun Mar 28 08:32:27 2010
 */
/* Compiler settings for .\RDH2LEGODriver.idl:
    Oicf, W1, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __RDH2LEGODriver_h_h__
#define __RDH2LEGODriver_h_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDriver_FWD_DEFINED__
#define __IDriver_FWD_DEFINED__
typedef interface IDriver IDriver;
#endif 	/* __IDriver_FWD_DEFINED__ */


#ifndef __Driver_FWD_DEFINED__
#define __Driver_FWD_DEFINED__

#ifdef __cplusplus
typedef class Driver Driver;
#else
typedef struct Driver Driver;
#endif /* __cplusplus */

#endif 	/* __Driver_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IDriver_INTERFACE_DEFINED__
#define __IDriver_INTERFACE_DEFINED__

/* interface IDriver */
/* [unique][helpstring][nonextensible][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IDriver;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C7B59BD7-9B75-4BD7-BB01-0EE73611B246")
    IDriver : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDriverVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDriver * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDriver * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDriver * This);
        
        END_INTERFACE
    } IDriverVtbl;

    interface IDriver
    {
        CONST_VTBL struct IDriverVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDriver_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDriver_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDriver_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDriver_INTERFACE_DEFINED__ */



#ifndef __RDH2LEGODriverLib_LIBRARY_DEFINED__
#define __RDH2LEGODriverLib_LIBRARY_DEFINED__

/* library RDH2LEGODriverLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_RDH2LEGODriverLib;

EXTERN_C const CLSID CLSID_Driver;

#ifdef __cplusplus

class DECLSPEC_UUID("7C4AD20F-EAB1-48D2-A92F-691EA9F92FF5")
Driver;
#endif
#endif /* __RDH2LEGODriverLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


