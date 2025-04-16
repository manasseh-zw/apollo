import { redirect } from "@tanstack/react-router";
import { store } from "../state/store";

const AUTH_CHECK_TIMEOUT = 5000; // 5 seconds timeout

async function waitForAuthState(): Promise<void> {
  return new Promise((resolve, reject) => {
    const timeout = setTimeout(() => {
      reject(new Error("Auth state check timed out"));
    }, AUTH_CHECK_TIMEOUT);

    // Check immediately first
    if (!store.state.authState.isLoading) {
      clearTimeout(timeout);
      resolve();
      return;
    }

    const unsubscribe = store.subscribe((state) => {
      if (!state.currentVal.authState.isLoading) {
        clearTimeout(timeout);
        unsubscribe();
        resolve();
      }
    });

    return () => {
      clearTimeout(timeout);
      unsubscribe();
    };
    
  });
}

/**
 * A loader function that checks if the user is authenticated
 * and redirects to the sign-in page if not
 */
export async function protectedLoader() {
  try {
    await waitForAuthState();
    
    const { isAuthenticated } = store.state.authState;
    if (!isAuthenticated) {
      throw redirect({
        to: "/auth/sign-in",
        search: {
          redirect: window.location.pathname,
        },
      });
    }

    return null;
  } catch (error) {
    // If timeout or other error, redirect to sign-in as a fallback
    throw redirect({
      to: "/auth/sign-in",
      search: {
        redirect: window.location.pathname,
      },
    });
  }
}

/**
 * A loader function that checks if the user is authenticated
 * and redirects to the dashboard if they are
 */
export async function publicOnlyLoader() {
  try {
    await waitForAuthState();
    
    const { isAuthenticated } = store.state.authState;
    if (isAuthenticated) {
      throw redirect({
        to: "/research",
      });
    }

    return null;
  } catch (error) {
    // If timeout or other error, allow access to public route
    return null;
  }
}
