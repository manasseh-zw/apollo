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
