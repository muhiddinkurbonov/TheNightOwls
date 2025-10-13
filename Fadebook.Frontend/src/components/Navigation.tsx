'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useAuth } from '@/providers/AuthProvider';
import { Button } from '@/components/ui/button';
import { ThemeToggle } from '@/components/ThemeToggle';

export function Navigation() {
  const pathname = usePathname();
  const { user, isAuthenticated, logout } = useAuth();
  const [mounted, setMounted] = useState(false);

  // Avoid hydration mismatch by only rendering user-dependent UI after mount
  useEffect(() => {
    setMounted(true);
  }, []);

  // Role-based home link - best practice for multi-role applications
  const getHomeLink = () => {
    if (!user) return '/';

    switch (user.role) {
      case 'Admin':
        return '/admin';
      case 'Barber':
        return '/barbers';
      case 'Customer':
      default:
        return '/';
    }
  };

  const links = [
    { href: '/', label: 'Home', excludeAdmin: true, excludeBarber: true },
    { href: '/my-appointments', label: 'My Appointments', protected: true, customerOnly: true },
    { href: '/barbers', label: 'Barbers', excludeAdmin: true, excludeBarber: true },
    { href: '/book', label: 'Book Appointment', protected: true, customerOnly: true },
    { href: '/barber-dashboard', label: 'Barber Dashboard', barberOnly: true },
    { href: '/admin', label: 'Admin Dashboard', adminOnly: true },
  ];

  return (
    <nav className="border-b bg-background">
      <div className="container mx-auto px-2 sm:px-4">
        <div className="flex h-14 sm:h-16 items-center justify-between">
          <div className="flex items-center gap-1 sm:gap-2">
            <Link href={getHomeLink()} className="flex items-center text-lg sm:text-xl font-bold gap-1">
              <img
                src="/FadeBook Logo with Razor Icon.svg"
                alt="FadeBook logo"
                className="w-16 h-16 sm:w-24 sm:h-24 object-contain mr-1 dark:invert dark:brightness-0 dark:contrast-200"
                loading="eager"
              />
            </Link>
            <div className="hidden md:flex gap-2 lg:gap-4">
              {mounted && links.map((link) => {
                // Hide protected links if not authenticated
                if (link.protected && !isAuthenticated) return null;

                // Hide admin-only links for non-admins
                if (link.adminOnly && user?.role !== 'Admin') return null;

                // Hide barber-only links for non-barbers
                if (link.barberOnly && user?.role !== 'Barber') return null;

                // Hide customer-only links for admins and barbers
                if (link.customerOnly && user?.role !== 'Customer') return null;

                // Hide links that should be excluded for admins
                if (link.excludeAdmin && user?.role === 'Admin') return null;

                // Hide links that should be excluded for barbers
                if (link.excludeBarber && user?.role === 'Barber') return null;

                return (
                  <Link
                    key={link.href}
                    href={link.href}
                    className={`px-2 lg:px-3 py-2 rounded-md text-xs lg:text-sm font-medium transition-colors ${
                      pathname === link.href
                        ? 'bg-primary text-primary-foreground'
                        : 'text-muted-foreground hover:bg-accent hover:text-accent-foreground'
                    }`}
                  >
                    {link.label}
                  </Link>
                );
              })}
            </div>
          </div>

          <div className="flex items-center gap-1 sm:gap-2 md:gap-4">
            <ThemeToggle />
            {!mounted ? (
              // Render placeholder during SSR/initial hydration to match server HTML
              <>
                <Link href="/signin">
                  <Button variant="ghost" size="sm" className="text-xs sm:text-sm px-2 sm:px-3">
                    Sign In
                  </Button>
                </Link>
                <Link href="/signup">
                  <Button size="sm" className="text-xs sm:text-sm px-2 sm:px-3">
                    Sign Up
                  </Button>
                </Link>
              </>
            ) : isAuthenticated && user ? (
              <>
                <span className="hidden sm:inline text-xs sm:text-sm text-muted-foreground">
                  Welcome, <span className="font-medium text-foreground">{user.name}</span>
                </span>
                <Button variant="outline" size="sm" onClick={logout} className="text-xs sm:text-sm">
                  Sign Out
                </Button>
              </>
            ) : (
              <>
                <Link href="/signin">
                  <Button variant="ghost" size="sm" className="text-xs sm:text-sm px-2 sm:px-3">
                    Sign In
                  </Button>
                </Link>
                <Link href="/signup">
                  <Button size="sm" className="text-xs sm:text-sm px-2 sm:px-3">
                    Sign Up
                  </Button>
                </Link>
              </>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}
