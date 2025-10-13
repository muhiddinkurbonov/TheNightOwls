'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Navigation } from '@/components/Navigation';
import { useAuth } from '@/providers/AuthProvider';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { BarbersTab } from '@/components/admin/BarbersTab';
import { CustomersTab } from '@/components/admin/CustomersTab';
import { AppointmentsTab } from '@/components/admin/AppointmentsTab';
import { ServicesTab } from '@/components/admin/ServicesTab';
import { UsersTab } from '@/components/admin/UsersTab';
import { WorkHoursTab } from '@/components/admin/WorkHoursTab';

export default function AdminPage() {
  const router = useRouter();
  const { user, isAuthenticated, isLoading } = useAuth();

  useEffect(() => {
    // Wait for auth to load
    if (isLoading) return;

    // Redirect if not authenticated
    if (!isAuthenticated) {
      router.push('/signin');
      return;
    }

    // Redirect if not admin
    if (user && user.role !== 'Admin') {
      router.push('/book'); // Redirect non-admins to book page
      return;
    }
  }, [user, isAuthenticated, isLoading, router]);

  // Show loading state while checking auth
  if (isLoading || !user || user.role !== 'Admin') {
    return (
      <div className="min-h-screen flex flex-col">
        <Navigation />
        <main className="flex-1 py-8 px-4">
          <div className="container mx-auto">
            <p className="text-center text-muted-foreground">Loading...</p>
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex flex-col">
      <Navigation />
      <main className="flex-1 py-8 px-4">
        <div className="container mx-auto">
          <div className="mb-8">
            <h1 className="text-4xl font-bold mb-2">Admin Dashboard</h1>
            <p className="text-muted-foreground">
              Manage barbers, services, customers, and appointments
            </p>
          </div>

          <Tabs defaultValue="users" className="space-y-6">
            <TabsList className="grid w-full grid-cols-2 sm:grid-cols-3 md:grid-cols-6 gap-2 h-auto p-2">
              <TabsTrigger value="users">Users</TabsTrigger>
              <TabsTrigger value="barbers">Barbers</TabsTrigger>
              <TabsTrigger value="services">Services</TabsTrigger>
              <TabsTrigger value="customers">Customers</TabsTrigger>
              <TabsTrigger value="appointments">Appointments</TabsTrigger>
              <TabsTrigger value="workHours">Work Hours</TabsTrigger>
            </TabsList>

            <TabsContent value="users">
              <UsersTab />
            </TabsContent>

            <TabsContent value="barbers">
              <BarbersTab />
            </TabsContent>

            <TabsContent value="services">
              <ServicesTab />
            </TabsContent>

            <TabsContent value="customers">
              <CustomersTab />
            </TabsContent>

            <TabsContent value="appointments">
              <AppointmentsTab />
            </TabsContent>

            <TabsContent value="workHours">
              <WorkHoursTab />
            </TabsContent>
          </Tabs>
        </div>
      </main>
    </div>
  );
}
