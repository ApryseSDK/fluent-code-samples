package com.example.customprocessing;

import com.windward.restfulengine.customprocessing.IDocumentPostProcessor;
import com.windward.restfulengine.customprocessing.CustomDocument;

public class CustomProcessor implements IDocumentPostProcessor {
    // This is the only method that needs to be defined in the IDocumentPostProcessor.
    // You can implement any other helper methods, but this is the method the RESTful engine will call.
    @Override
    public CustomDocument process(CustomDocument document) {
        // Setting the document data to an empty byte array
        document.setData(new byte[0]);
        return document;
    }
}
