/** @type {import('tailwindcss').Config} */

import { heroui } from "@heroui/react";

export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
    "./node_modules/@heroui/theme/dist/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      fontFamily: {
        rubik: ["Rubik", "serif"],
        geist: ["Geist", "sans-serif"],
      },
    },
  },
  darkMode: "class",
  plugins: [
    heroui({
        themes: {
 
          light: { // Updated Light Theme with new Grayscale
            colors: {
              // --- Grayscale Palette Applied ---
              default: {
                50: "#f8f9fa",    
                100: "#e9ecef",   
                200: "#dee2e6",   
                300: "#ced4da",   
                400: "#adb5bd",   
                500: "#6c757d",  
                600: "#495057", 
                700: "#343a40", 
                800: "#212529", 
                900: "#212529", 
                foreground: "#212529",
                DEFAULT: "#6c757d", 
              },
              primary: {
                50: "#f8f9fa",
                100: "#e9ecef",
                200: "#dee2e6",
                300: "#ced4da",
                400: "#adb5bd",
                500: "#6c757d",
                600: "#495057",
                700: "#343a40",       
                800: "#252525",       
                900: "#1a1a1a",       
                foreground: "#f8f9fa", 
                DEFAULT: "#252525",   
              },              
              secondary: {
                50: "#f8f9fa",
                100: "#e9ecef",
                200: "#dee2e6",
                300: "#ced4da",
                400: "#adb5bd",   // French Gray 2 as secondary default base
                500: "#6c757d",
                600: "#495057",
                700: "#343a40",
                800: "#212529",
                900: "#212529",
                foreground: "#f8f9fa", // Lightest gray for text on secondary
                DEFAULT: "#adb5bd",   // French Gray 2 as secondary default
              },
              background: "#f8f9fa", // Lightest gray for background
              foreground: { // Shades for text/icons if needed
                50: "#f8f9fa",
                100: "#e9ecef",
                200: "#dee2e6",
                300: "#ced4da",
                400: "#adb5bd",
                500: "#6c757d",
                600: "#495057",
                700: "#343a40",
                800: "#212529", // Darkest gray as default foreground text
                900: "#212529",
                foreground: "#f8f9fa", // Contrast for the foreground color itself (if used as bg)
                DEFAULT: "#212529",    // Darkest gray as default foreground text
              },
              content1: {
                DEFAULT: "#f8f9fa",    // Seasalt
                foreground: "#212529", // Dark text
              },
              content2: {
                DEFAULT: "#e9ecef",   // Antiflash White
                foreground: "#212529", // Dark text
              },
              content3: {
                DEFAULT: "#dee2e6",   // Platinum
                foreground: "#212529", // Dark text
              },
              content4: {
                DEFAULT: "#ced4da",   // French Gray
                foreground: "#212529", // Dark text
              },
              focus: "#6c757d",       // Slate Gray for focus rings
              overlay: "#212529",       // Dark overlay
              divider: "#dee2e6",     // Platinum for dividers
        
              // --- Kept Original Colors ---
              success: {
                50: "#f3f6f0",
                100: "#e2e9db",
                200: "#d0ddc6",
                300: "#bfd0b1",
                400: "#aec49c",
                500: "#9db787",
                600: "#82976f",
                700: "#667758",
                800: "#4b5740",
                900: "#2f3729",
                foreground: "#000",
                DEFAULT: "#9db787",
              },
              warning: {
                50: "#fff9eb",
                100: "#fff2cf",
                200: "#ffeab3",
                300: "#ffe297",
                400: "#ffda7b",
                500: "#ffd25f",
                600: "#d2ad4e",
                700: "#a6893e",
                800: "#79642d",
                900: "#4d3f1d",
                foreground: "#000",
                DEFAULT: "#ffd25f",
              },
              danger: {
                50: "#fff2ef",
                100: "#fedfd9",
                200: "#fecdc3",
                300: "#fdbaad",
                400: "#fda897",
                500: "#fc9581",
                600: "#d07b6a",
                700: "#a46154",
                800: "#78473d",
                900: "#4c2d27",
                foreground: "#000",
                DEFAULT: "#fc9581",
              },
            },
          },
          dark: { // New Dark Theme based on Grayscale
            colors: {
              // --- Grayscale Palette Applied (inverted/darker bias) ---
              default: {
                50: "#212529",    // --eerie-black
                100: "#343a40",   // --onyx
                200: "#495057",   // --outer-space
                300: "#6c757d",   // --slate-gray
                400: "#adb5bd",   // --french-gray-2
                500: "#ced4da",   // --french-gray
                600: "#dee2e6",   // --platinum
                700: "#e9ecef",   // --antiflash-white
                800: "#f8f9fa",   // --seasalt
                900: "#f8f9fa",   // Re-using lightest for 900
                foreground: "#f8f9fa", // Lightest gray for text
                DEFAULT: "#6c757d", // Slate Gray as default
              },
              primary: {
                50: "#212529",
                100: "#343a40",
                200: "#495057",
                300: "#6c757d",
                400: "#adb5bd",
                500: "#ced4da",   // French Gray as primary default base
                600: "#dee2e6",
                700: "#e9ecef",
                800: "#f8f9fa",
                900: "#f8f9fa",
                foreground: "#212529", // Darkest gray for text on primary
                DEFAULT: "#ced4da",    // French Gray as primary default
              },
              secondary: {
                50: "#212529",
                100: "#343a40",
                200: "#495057",   // Outer Space as secondary default base
                300: "#6c757d",
                400: "#adb5bd",
                500: "#ced4da",
                600: "#dee2e6",
                700: "#e9ecef",
                800: "#f8f9fa",
                900: "#f8f9fa",
                foreground: "#f8f9fa", // Lightest gray for text on secondary
                DEFAULT: "#495057",   // Outer Space as secondary default
              },
              background: "#212529", // Darkest gray for background
              foreground: { // Shades for text/icons if needed
                50: "#212529",
                100: "#343a40",
                200: "#495057",
                300: "#6c757d",
                400: "#adb5bd",
                500: "#ced4da",
                600: "#dee2e6",
                700: "#e9ecef",
                800: "#f8f9fa",    // Lightest gray as default foreground text
                900: "#f8f9fa",
                foreground: "#212529", // Contrast for the foreground color itself (if used as bg)
                DEFAULT: "#f8f9fa",    // Lightest gray as default foreground text
              },
              content1: {
                DEFAULT: "#212529",    // Eerie Black
                foreground: "#f8f9fa", // Light text
              },
              content2: {
                DEFAULT: "#343a40",   // Onyx
                foreground: "#f8f9fa", // Light text
              },
              content3: {
                DEFAULT: "#495057",   // Outer Space
                foreground: "#f8f9fa", // Light text
              },
              content4: {
                DEFAULT: "#6c757d",   // Slate Gray
                foreground: "#f8f9fa", // Light text
              },
              focus: "#adb5bd",       // French Gray 2 for focus rings
              overlay: "#f8f9fa",       // Light overlay
              divider: "#495057",     // Outer space for dividers
        
              // --- Kept Original Dark Theme Colors ---
              success: { // Copied from original dark theme
                50: "#003321",
                100: "#005034",
                200: "#006e48",
                300: "#008b5b",
                400: "#00a96e",
                500: "#2db887",
                600: "#59c7a1",
                700: "#86d6ba",
                800: "#b3e5d4",
                900: "#dff4ed",
                foreground: "#000", // Should contrast with DEFAULT; might need adjustment based on usage
                DEFAULT: "#00a96e",
              },
              warning: { // Copied from original dark theme
                50: "#4d3900",
                100: "#795a00",
                200: "#a67c00",
                300: "#d29d00",
                400: "#ffbe00",
                500: "#ffc92d",
                600: "#ffd559",
                700: "#ffe086",
                800: "#ffecb3",
                900: "#fff7df",
                foreground: "#000", // Should contrast with DEFAULT
                DEFAULT: "#ffbe00",
              },
              danger: { // Copied from original dark theme
                50: "#4d1a1d",
                100: "#792a2e",
                200: "#a6393f",
                300: "#d24950",
                400: "#ff5861",
                500: "#ff757d",
                600: "#ff9298",
                700: "#ffb0b4",
                800: "#ffcdd0",
                900: "#ffeaeb",
                foreground: "#000", // Should contrast with DEFAULT
                DEFAULT: "#ff5861",
              },
            },
          },
        },
      layout: {
        fontSize: {
          tiny: "0.75rem",
          small: "0.875rem",
          medium: "1rem",
          large: "1.125rem",
        },
        lineHeight: {
          tiny: "1rem",
          small: "1.25rem",
          medium: "1.5rem",
          large: "1.75rem",
        },
        radius: {
          small: "0.5rem",
          medium: "0.8rem",
          large: "0.975rem",
        },
        borderWidth: {
          small: "1px",
          medium: "2px",
          large: "3px",
        },
        disabledOpacity: "0.5",
        dividerWeight: "1",
        hoverOpacity: "0.9",
      },
    }),
    require("@tailwindcss/typography"),
    require("tailwindcss-animate"),
  ],
};
