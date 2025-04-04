import { SelectOption } from "@/types/location.types";
import { Select } from "./Select";
import { LocationIcon } from "../icons";

export const LocationPicker = (props: Props) => {
  return (
    <Select
      items={props.locationList.map((location) => ({
        id: location.id,
        label: location.address,
      }))}
      placeholder="Location"
      value={props.locationId}
      setValue={props.setLocation}
      className="w-full"
      Icon={() => <LocationIcon />}
      loading={props.loading}
      disabled={props.disabled}
    />
  );
};

interface Props {
  locationList: SelectOption[];
  locationId: SelectOption["id"];
  setLocation: (id: SelectOption["id"]) => void;
  loading?: boolean;
  disabled?: boolean;
}
