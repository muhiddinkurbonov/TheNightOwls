'use client';

import { useEffect, useState } from 'react';
import { useAuth } from '@/providers/AuthProvider';
import { workHoursApi } from '@/lib/api/workHours';
import type { BarberWorkHoursDto, CreateBarberWorkHoursDto } from '@/types/api';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Switch } from '@/components/ui/switch';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Plus, Edit, Trash2, Clock, Calendar } from 'lucide-react';
import { toast } from 'sonner';

const DAYS_OF_WEEK = [
  { value: 0, label: 'Sunday' },
  { value: 1, label: 'Monday' },
  { value: 2, label: 'Tuesday' },
  { value: 3, label: 'Wednesday' },
  { value: 4, label: 'Thursday' },
  { value: 5, label: 'Friday' },
  { value: 6, label: 'Saturday' },
];

export function BarberAvailabilityTab() {
  const { user } = useAuth();
  const [workHours, setWorkHours] = useState<BarberWorkHoursDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [isAddDialogOpen, setIsAddDialogOpen] = useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [editingWorkHour, setEditingWorkHour] = useState<BarberWorkHoursDto | null>(null);
  const [deletingWorkHour, setDeletingWorkHour] = useState<BarberWorkHoursDto | null>(null);

  // Form state
  const [formData, setFormData] = useState<CreateBarberWorkHoursDto>({
    barberId: user?.barberId || 0,
    dayOfWeek: 1, // Monday
    startTime: '09:00',
    endTime: '17:00',
    isActive: true,
  });

  useEffect(() => {
    if (user?.barberId) {
      loadWorkHours();
    }
  }, [user?.barberId]);

  const loadWorkHours = async () => {
    if (!user?.barberId) return;

    try {
      setLoading(true);
      const data = await workHoursApi.getByBarberId(user.barberId);
      setWorkHours(data);
    } catch (error) {
      console.error('Failed to load work hours:', error);
      toast.error('Failed to load your availability');
    } finally {
      setLoading(false);
    }
  };

  const handleAdd = async () => {
    if (!user?.barberId) return;

    try {
      const dataToSubmit = { ...formData, barberId: user.barberId };
      await workHoursApi.create(dataToSubmit);
      toast.success('Work hours added successfully');
      setIsAddDialogOpen(false);
      resetForm();
      loadWorkHours();
    } catch (error: any) {
      console.error('Failed to add work hours:', error);
      toast.error(error.response?.data?.message || 'Failed to add work hours');
    }
  };

  const handleEdit = async () => {
    if (!editingWorkHour || !user?.barberId) return;

    try {
      const dataToSubmit = { ...formData, barberId: user.barberId };
      await workHoursApi.update(editingWorkHour.workHourId, dataToSubmit);
      toast.success('Work hours updated successfully');
      setIsEditDialogOpen(false);
      setEditingWorkHour(null);
      resetForm();
      loadWorkHours();
    } catch (error: any) {
      console.error('Failed to update work hours:', error);
      toast.error(error.response?.data?.message || 'Failed to update work hours');
    }
  };

  const openDeleteDialog = (workHour: BarberWorkHoursDto) => {
    setDeletingWorkHour(workHour);
    setIsDeleteDialogOpen(true);
  };

  const handleDelete = async () => {
    if (!deletingWorkHour) return;

    try {
      await workHoursApi.delete(deletingWorkHour.workHourId);
      toast.success('Work hours deleted successfully');
      setIsDeleteDialogOpen(false);
      setDeletingWorkHour(null);
      loadWorkHours();
    } catch (error: any) {
      console.error('Failed to delete work hours:', error);
      toast.error(error.response?.data?.message || 'Failed to delete work hours');
    }
  };

  const resetForm = () => {
    setFormData({
      barberId: user?.barberId || 0,
      dayOfWeek: 1,
      startTime: '09:00',
      endTime: '17:00',
      isActive: true,
    });
  };

  const openEditDialog = (workHour: BarberWorkHoursDto) => {
    setEditingWorkHour(workHour);
    setFormData({
      barberId: workHour.barberId,
      dayOfWeek: workHour.dayOfWeek,
      startTime: workHour.startTime,
      endTime: workHour.endTime,
      isActive: workHour.isActive,
    });
    setIsEditDialogOpen(true);
  };

  // Group work hours by day
  const workHoursByDay = DAYS_OF_WEEK.map((day) => ({
    ...day,
    hours: workHours.filter((wh) => wh.dayOfWeek === day.value),
  }));

  if (!user?.barberId) {
    return (
      <Card>
        <CardContent className="py-12">
          <p className="text-center text-muted-foreground">No barber profile found</p>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle>My Availability</CardTitle>
              <CardDescription>Manage your working hours for each day</CardDescription>
            </div>
            <Dialog open={isAddDialogOpen} onOpenChange={setIsAddDialogOpen}>
              <DialogTrigger asChild>
                <Button onClick={resetForm}>
                  <Plus className="h-4 w-4 mr-2" />
                  Add Work Hours
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Add Work Hours</DialogTitle>
                  <DialogDescription>
                    Add a new time block to your schedule
                  </DialogDescription>
                </DialogHeader>
                <div className="space-y-4 py-4">
                  <div className="space-y-2">
                    <Label>Day of Week</Label>
                    <Select
                      value={formData.dayOfWeek.toString()}
                      onValueChange={(value) =>
                        setFormData({ ...formData, dayOfWeek: parseInt(value) })
                      }
                    >
                      <SelectTrigger>
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        {DAYS_OF_WEEK.map((day) => (
                          <SelectItem key={day.value} value={day.value.toString()}>
                            {day.label}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>

                  <div className="grid grid-cols-2 gap-4">
                    <div className="space-y-2">
                      <Label>Start Time</Label>
                      <Input
                        type="time"
                        value={formData.startTime}
                        onChange={(e) =>
                          setFormData({ ...formData, startTime: e.target.value })
                        }
                      />
                    </div>
                    <div className="space-y-2">
                      <Label>End Time</Label>
                      <Input
                        type="time"
                        value={formData.endTime}
                        onChange={(e) =>
                          setFormData({ ...formData, endTime: e.target.value })
                        }
                      />
                    </div>
                  </div>

                  <div className="flex items-center space-x-2">
                    <Switch
                      checked={formData.isActive}
                      onCheckedChange={(checked) =>
                        setFormData({ ...formData, isActive: checked })
                      }
                    />
                    <Label>Active</Label>
                  </div>
                </div>
                <DialogFooter>
                  <Button variant="outline" onClick={() => setIsAddDialogOpen(false)}>
                    Cancel
                  </Button>
                  <Button onClick={handleAdd}>Add</Button>
                </DialogFooter>
              </DialogContent>
            </Dialog>
          </div>
        </CardHeader>
        <CardContent>
          {loading ? (
            <p className="text-center text-muted-foreground py-8">Loading...</p>
          ) : (
            <div className="space-y-4">
              {workHoursByDay.map((day) => (
                <div
                  key={day.value}
                  className="border rounded-lg p-4 hover:bg-muted/50 transition-colors"
                >
                  <div className="flex items-center justify-between mb-3">
                    <div className="flex items-center gap-2">
                      <Calendar className="h-4 w-4 text-muted-foreground" />
                      <h3 className="font-semibold">{day.label}</h3>
                      {day.hours.length > 0 && (
                        <Badge variant="secondary">{day.hours.length} blocks</Badge>
                      )}
                    </div>
                  </div>

                  {day.hours.length === 0 ? (
                    <p className="text-sm text-muted-foreground">No work hours set</p>
                  ) : (
                    <div className="space-y-2">
                      {day.hours.map((wh) => (
                        <div
                          key={wh.workHourId}
                          className="flex items-center justify-between p-3 bg-background rounded border"
                        >
                          <div className="flex items-center gap-3">
                            <Clock className="h-4 w-4 text-muted-foreground" />
                            <span className="font-medium">
                              {wh.startTime} - {wh.endTime}
                            </span>
                            <Badge variant={wh.isActive ? 'default' : 'secondary'}>
                              {wh.isActive ? 'Active' : 'Inactive'}
                            </Badge>
                          </div>
                          <div className="flex gap-2">
                            <Button
                              variant="ghost"
                              size="icon"
                              onClick={() => openEditDialog(wh)}
                            >
                              <Edit className="h-4 w-4" />
                            </Button>
                            <Button
                              variant="ghost"
                              size="icon"
                              onClick={() => openDeleteDialog(wh)}
                            >
                              <Trash2 className="h-4 w-4 text-destructive" />
                            </Button>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      {/* Edit Dialog */}
      <Dialog open={isEditDialogOpen} onOpenChange={setIsEditDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Edit Work Hours</DialogTitle>
            <DialogDescription>Update your work hours</DialogDescription>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <Label>Day of Week</Label>
              <Select
                value={formData.dayOfWeek.toString()}
                onValueChange={(value) =>
                  setFormData({ ...formData, dayOfWeek: parseInt(value) })
                }
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {DAYS_OF_WEEK.map((day) => (
                    <SelectItem key={day.value} value={day.value.toString()}>
                      {day.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label>Start Time</Label>
                <Input
                  type="time"
                  value={formData.startTime}
                  onChange={(e) =>
                    setFormData({ ...formData, startTime: e.target.value })
                  }
                />
              </div>
              <div className="space-y-2">
                <Label>End Time</Label>
                <Input
                  type="time"
                  value={formData.endTime}
                  onChange={(e) =>
                    setFormData({ ...formData, endTime: e.target.value })
                  }
                />
              </div>
            </div>

            <div className="flex items-center space-x-2">
              <Switch
                checked={formData.isActive}
                onCheckedChange={(checked) =>
                  setFormData({ ...formData, isActive: checked })
                }
              />
              <Label>Active</Label>
            </div>
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => {
                setIsEditDialogOpen(false);
                setEditingWorkHour(null);
                resetForm();
              }}
            >
              Cancel
            </Button>
            <Button onClick={handleEdit}>Save Changes</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Delete Confirmation Dialog */}
      <Dialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <DialogContent className="max-w-md">
          <DialogHeader>
            <DialogTitle>Delete Work Hours</DialogTitle>
            <DialogDescription>
              Are you sure you want to delete the work hours for{' '}
              <strong>{deletingWorkHour?.dayOfWeekName || DAYS_OF_WEEK.find(d => d.value === deletingWorkHour?.dayOfWeek)?.label}</strong>{' '}
              from <strong>{deletingWorkHour?.startTime}</strong> to <strong>{deletingWorkHour?.endTime}</strong>?
              <br />
              <br />
              This action cannot be undone.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter className="gap-2 sm:gap-0">
            <Button
              variant="outline"
              onClick={() => {
                setIsDeleteDialogOpen(false);
                setDeletingWorkHour(null);
              }}
            >
              Cancel
            </Button>
            <Button variant="destructive" onClick={handleDelete}>
              Delete
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
