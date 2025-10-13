'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { Navigation } from '@/components/Navigation';
import { useAuth } from '@/providers/AuthProvider';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { BarberAppointmentsTab } from '@/components/barber/BarberAppointmentsTab';
import { BarberProfileTab } from '@/components/barber/BarberProfileTab';
import { BarberAvailabilityTab } from '@/components/barber/BarberAvailabilityTab';

export default function BarberDashboardPage() {
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

    // Redirect if not barber
    if (user && user.role !== 'Barber') {
      router.push('/book'); // Redirect non-barbers to book page
      return;
    }
  }, [user, isAuthenticated, isLoading, router]);

  // Show loading state while checking auth
  if (isLoading || !user || user.role !== 'Barber') {
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
            <h1 className="text-4xl font-bold mb-2">Barber Dashboard</h1>
            <p className="text-muted-foreground">
              Manage your appointments, availability, and profile
            </p>
          </div>

          <Tabs defaultValue="appointments" className="space-y-6">
            <TabsList className="grid w-full grid-cols-3 max-w-2xl">
              <TabsTrigger value="appointments">My Appointments</TabsTrigger>
              <TabsTrigger value="availability">My Availability</TabsTrigger>
              <TabsTrigger value="profile">My Profile</TabsTrigger>
            </TabsList>

            <TabsContent value="appointments">
              <BarberAppointmentsTab />
            </TabsContent>

            <TabsContent value="availability">
              <BarberAvailabilityTab />
            </TabsContent>

            <TabsContent value="profile">
              <BarberProfileTab />
            </TabsContent>
          </Tabs>
        </div>
      </main>
    </div>
  );
}
