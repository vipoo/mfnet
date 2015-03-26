// Add these lines to CreateContentInfo in Splitter sample
#if false
            int i;
            IMFMediaBuffer pb;
            IMFPresentationDescriptor pd;
            IPropertyStore ps;

            hr = ppContentInfo.GenerateHeader(null, out i);
            MFError.ThrowExceptionForHR(hr);
            hr = MFExtern.MFCreateMemoryBuffer(i, out pb);
            MFError.ThrowExceptionForHR(hr);
            hr = ppContentInfo.GenerateHeader(pb, out i);
            MFError.ThrowExceptionForHR(hr);
            hr = ppContentInfo.GeneratePresentationDescriptor(out pd);
            MFError.ThrowExceptionForHR(hr);
            hr = ppContentInfo.GetEncodingConfigurationPropertyStore(1, out ps);
            MFError.ThrowExceptionForHR(hr);
#endif
