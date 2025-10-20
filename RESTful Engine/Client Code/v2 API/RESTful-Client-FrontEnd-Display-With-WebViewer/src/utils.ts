import fs from 'fs/promises';
import path from 'path';
import { Buffer } from 'buffer';
import fetch from 'node-fetch';

// Helper function to convert file content to base64
export async function getContentAsBase64(filePathOrUrl: string): Promise<string> {
  // Check if it's a remote URL
  if (filePathOrUrl.startsWith('http://') || filePathOrUrl.startsWith('https://')) {
    try {
      const response = await fetch(filePathOrUrl);
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      // Get the response body as an ArrayBuffer
      const arrayBuffer = await response.arrayBuffer();
      // Convert ArrayBuffer to a Node.js Buffer
      const buffer = Buffer.from(arrayBuffer);
      // Convert Buffer to base64 string
      return buffer.toString('base64');
    } catch (error) {
      console.error('Error fetching remote template:', error);
      throw error;
    }
  } else {
    // Handle as a local file path
    try {
      // Resolve the relative path to an absolute one
      const absolutePath = path.resolve(filePathOrUrl);
      console.log('Resolved local file path:', absolutePath);
      // Read the file directly into a base64 string
      const base64String = await fs.readFile(absolutePath, { encoding: 'base64' });
      return base64String;
    } catch (error) {
      console.error('Error reading local template:', error);
      throw error;
    }
  }
}