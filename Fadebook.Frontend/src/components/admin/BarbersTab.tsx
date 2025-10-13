'use client';

import { useState, useEffect } from 'react';
import { useBarbers, useCreateBarber, useUpdateBarber, useDeleteBarber, useUpdateBarberServices } from '@/hooks/useBarbers';
import { useServices } from '@/hooks/useCustomers';
import { useBarberServices } from '@/hooks/useServices';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Badge } from '@/components/ui/badge';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Checkbox } from '@/components/ui/checkbox';
import { Plus, Trash2, Pencil } from 'lucide-react';
import type { CreateBarberDto, BarberDto } from '@/types/api';

export function BarbersTab() {
  const { data: barbers, isLoading, error } = useBarbers();
  const { data: services } = useServices();
  const createBarber = useCreateBarber();
  const updateBarber = useUpdateBarber();
  const updateBarberServices = useUpdateBarberServices();
  const deleteBarber = useDeleteBarber();
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [editingBarber, setEditingBarber] = useState<BarberDto | null>(null);
  const [deletingBarber, setDeletingBarber] = useState<BarberDto | null>(null);
  const [formData, setFormData] = useState({
    username: '',
    name: '',
    specialty: '',
    contactInfo: '',
    serviceIds: [] as number[],
  });

  // Fetch barber's services when editing
  const { data: barberServices } = useBarberServices(editingBarber?.barberId || 0);

  // Load barber's services when they're fetched
  useEffect(() => {
    if (editingBarber && barberServices) {
      setFormData(prev => ({
        ...prev,
        serviceIds: barberServices.map(s => s.serviceId)
      }));
    }
  }, [editingBarber, barberServices]);

  const handleOpenDialog = (barber?: BarberDto) => {
    if (barber) {
      setEditingBarber(barber);
      setFormData({
        username: barber.username,
        name: barber.name,
        specialty: barber.specialty || '',
        contactInfo: barber.contactInfo || '',
        serviceIds: [], // Will be populated by useEffect when barberServices loads
      });
    } else {
      setEditingBarber(null);
      setFormData({
        username: '',
        name: '',
        specialty: '',
        contactInfo: '',
        serviceIds: [],
      });
    }
    setIsDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setIsDialogOpen(false);
    setEditingBarber(null);
    setFormData({
      username: '',
      name: '',
      specialty: '',
      contactInfo: '',
      serviceIds: [],
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const barberData: CreateBarberDto = {
      username: formData.username,
      name: formData.name,
      specialty: formData.specialty,
      contactInfo: formData.contactInfo,
      serviceIds: formData.serviceIds,
    };

    try {
      if (editingBarber) {
        // When editing, we need to call two endpoints:
        // 1. Update barber basic info
        await updateBarber.mutateAsync({ id: editingBarber.barberId, barber: barberData });
        // 2. Update barber services separately
        await updateBarberServices.mutateAsync({ id: editingBarber.barberId, serviceIds: formData.serviceIds });
      } else {
        // When creating, the backend handles services in one call
        await createBarber.mutateAsync(barberData);
      }
      handleCloseDialog();
    } catch (error) {
      console.error(`Failed to ${editingBarber ? 'update' : 'create'} barber:`, error);
    }
  };

  const handleOpenDeleteDialog = (barber: BarberDto) => {
    setDeletingBarber(barber);
    setIsDeleteDialogOpen(true);
  };

  const handleCloseDeleteDialog = () => {
    setIsDeleteDialogOpen(false);
    setDeletingBarber(null);
  };

  const handleConfirmDelete = async () => {
    if (!deletingBarber) return;

    try {
      await deleteBarber.mutateAsync(deletingBarber.barberId);
      handleCloseDeleteDialog();
    } catch (error) {
      console.error('Failed to delete barber:', error);
    }
  };

  const toggleService = (serviceId: number) => {
    setFormData(prev => ({
      ...prev,
      serviceIds: prev.serviceIds.includes(serviceId)
        ? prev.serviceIds.filter(id => id !== serviceId)
        : [...prev.serviceIds, serviceId]
    }));
  };

  if (isLoading) {
    return (
      <Card>
        <CardContent className="py-12">
          <p className="text-center text-muted-foreground">Loading barbers...</p>
        </CardContent>
      </Card>
    );
  }

  if (error) {
    return (
      <Card>
        <CardContent className="py-12">
          <p className="text-center text-destructive">Error loading barbers</p>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <div>
            <CardTitle>Barbers</CardTitle>
            <CardDescription>Manage barbers and their services</CardDescription>
          </div>
          <Dialog open={isDialogOpen} onOpenChange={handleCloseDialog}>
            <DialogTrigger asChild>
              <Button onClick={() => handleOpenDialog()}>
                <Plus className="h-4 w-4 mr-2" />
                Add Barber
              </Button>
            </DialogTrigger>
            <DialogContent className="max-w-md">
              <DialogHeader>
                <DialogTitle>{editingBarber ? 'Edit Barber' : 'Add New Barber'}</DialogTitle>
                <DialogDescription>
                  {editingBarber ? 'Update barber information' : 'Create a new barber and assign services'}
                </DialogDescription>
              </DialogHeader>
              <form onSubmit={handleSubmit} className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="username">Username</Label>
                  <Input
                    id="username"
                    required
                    value={formData.username}
                    onChange={(e) =>
                      setFormData({ ...formData, username: e.target.value })
                    }
                    placeholder="barber-username"
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="name">Name</Label>
                  <Input
                    id="name"
                    required
                    value={formData.name}
                    onChange={(e) =>
                      setFormData({ ...formData, name: e.target.value })
                    }
                    placeholder="John Doe"
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="specialty">Specialty</Label>
                  <Input
                    id="specialty"
                    value={formData.specialty}
                    onChange={(e) =>
                      setFormData({ ...formData, specialty: e.target.value })
                    }
                    placeholder="Fades, Beards, etc."
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="contactInfo">Contact Info</Label>
                  <Input
                    id="contactInfo"
                    value={formData.contactInfo}
                    onChange={(e) =>
                      setFormData({ ...formData, contactInfo: e.target.value })
                    }
                    placeholder="Phone or email"
                  />
                </div>

                <div className="space-y-2">
                  <Label>Services</Label>
                  <div className="space-y-2 border rounded-md p-3 max-h-48 overflow-y-auto">
                    {services?.map((service) => (
                      <div key={service.serviceId} className="flex items-center space-x-2">
                        <Checkbox
                          id={`service-${service.serviceId}`}
                          checked={formData.serviceIds.includes(service.serviceId)}
                          onCheckedChange={() => toggleService(service.serviceId)}
                        />
                        <label
                          htmlFor={`service-${service.serviceId}`}
                          className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70 cursor-pointer"
                        >
                          {service.serviceName} - ${service.servicePrice}
                        </label>
                      </div>
                    ))}
                  </div>
                </div>

                <Button type="submit" className="w-full" disabled={createBarber.isPending || updateBarber.isPending || updateBarberServices.isPending}>
                  {editingBarber
                    ? (updateBarber.isPending || updateBarberServices.isPending ? 'Updating...' : 'Update Barber')
                    : (createBarber.isPending ? 'Creating...' : 'Create Barber')
                  }
                </Button>
              </form>
            </DialogContent>
          </Dialog>
        </div>
      </CardHeader>
      <CardContent>
        {barbers && barbers.length === 0 ? (
          <p className="text-center py-8 text-muted-foreground">No barbers found</p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead>Username</TableHead>
                <TableHead>Name</TableHead>
                <TableHead>Specialty</TableHead>
                <TableHead>Contact</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {barbers?.map((barber) => (
                <TableRow key={barber.barberId}>
                  <TableCell>{barber.barberId}</TableCell>
                  <TableCell>
                    <Badge variant="outline">{barber.username}</Badge>
                  </TableCell>
                  <TableCell className="font-medium">{barber.name}</TableCell>
                  <TableCell>{barber.specialty || '-'}</TableCell>
                  <TableCell>{barber.contactInfo || '-'}</TableCell>
                  <TableCell className="text-right">
                    <div className="flex justify-end gap-2">
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => handleOpenDialog(barber)}
                      >
                        <Pencil className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => handleOpenDeleteDialog(barber)}
                        disabled={deleteBarber.isPending}
                      >
                        <Trash2 className="h-4 w-4 text-destructive" />
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </CardContent>

      <Dialog open={isDeleteDialogOpen} onOpenChange={handleCloseDeleteDialog}>
        <DialogContent className="max-w-md">
          <DialogHeader>
            <DialogTitle>Delete Barber</DialogTitle>
            <DialogDescription>
              Are you sure you want to delete <strong>{deletingBarber?.name}</strong> (@{deletingBarber?.username})? This action cannot be undone and may affect existing appointments.
            </DialogDescription>
          </DialogHeader>
          <div className="flex justify-end gap-3 mt-4">
            <Button variant="outline" onClick={handleCloseDeleteDialog}>
              Cancel
            </Button>
            <Button
              variant="destructive"
              onClick={handleConfirmDelete}
              disabled={deleteBarber.isPending}
            >
              {deleteBarber.isPending ? 'Deleting...' : 'Delete Barber'}
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </Card>
  );
}
