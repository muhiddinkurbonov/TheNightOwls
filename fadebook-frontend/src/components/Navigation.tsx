'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useAuth } from '@/providers/AuthProvider';
import { Button } from '@/components/ui/button';
import { ThemeToggle } from '@/components/theme-toggle';

export function Navigation() {
  const pathname = usePathname();
  const { user, isAuthenticated, logout } = useAuth();

  const links = [
    { href: '/', label: 'Home' },
    { href: '/my-appointments', label: 'My Appointments', protected: true, customerOnly: true },
    { href: '/barbers', label: 'Barbers', excludeAdmin: true, excludeBarber: true },
    { href: '/book', label: 'Book Appointment', protected: true, customerOnly: true },
    { href: '/barber-dashboard', label: 'Barber Dashboard', barberOnly: true },
    { href: '/admin', label: 'Admin Dashboard', adminOnly: true },
  ];

  return (
    <nav className="border-b bg-background">
      <div className="container mx-auto px-4">
        <div className="flex h-16 items-center justify-between">
          <div className="flex items-center gap-2">
            <Link href="/" className="flex items-center text-xl font-bold gap-1">
              <img
                src="/FadeBook Logo with Razor Icon.svg"
                alt="FadeBook logo"
                className="w-24 h-24 object-contain mr-1 dark:invert dark:brightness-0 dark:contrast-200"
                loading="eager"
              />
            </Link>
            <div className="flex gap-4">
              {links.map((link) => {
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
                    className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
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

          <div className="flex items-center gap-4">
            <ThemeToggle />
            {isAuthenticated && user ? (
              <>
                <span className="text-sm text-muted-foreground">
                  Welcome, <span className="font-medium text-foreground">{user.name}</span>
                </span>
                <Button variant="outline" size="sm" onClick={logout}>
                  Sign Out
                </Button>
              </>
            ) : (
              <>
                <Link href="/signin">
                  <Button variant="ghost" size="sm">
                    Sign In
                  </Button>
                </Link>
                <Link href="/signup">
                  <Button size="sm">
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
