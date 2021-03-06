import React, { Fragment } from 'react';
import ListTimestamps from '../tables/ListTimestamps';
import FilterDateForm from '../../app/layout/FilterDateForm';
import { Button, Divider, Header } from 'semantic-ui-react';
import { useModalDispatch } from '../../app/context/modal/modalContext';
import { openModal } from '../../app/context/modal/modalActions';
import { useAuthState } from '../../app/context/auth/authContext';

import EditEmployee from './EditEmployee';
import DeleteEmployee from './DeleteEmployee';
import TimestampForm from '../timestamps/TimestampForm';
import EditManagerStatus from '../managers/EditManagerStatus';

// /employees/:username
const ViewEmployee = ({ match }) => {
  const username = match.params.username;
  const pageSize = 10;

  const modalDispatch = useModalDispatch();
  const { user } = useAuthState();

  return (
    <Fragment>
      <Header as='h2' color='teal'>
        Employee: {username}
      </Header>

      <Button
        className='add-employee-btn'
        color='teal'
        size='small'
        onClick={() =>
          openModal(<TimestampForm username={username} />, modalDispatch)
        }
      >
        Add Timestamp
      </Button>

      {user.admin && (
        <Button
          className='make-manager-btn'
          color='green'
          size='small'
          onClick={() =>
            openModal(
              <EditManagerStatus username={username} action='access' />,
              modalDispatch
            )
          }
        >
          Make Manager
        </Button>
      )}

      <Button
        className='edit-employee-btn'
        color='blue'
        size='small'
        onClick={() =>
          openModal(<EditEmployee username={username} />, modalDispatch)
        }
      >
        Edit Employee
      </Button>
      <Button
        className='delete-employee-btn'
        color='red'
        size='small'
        onClick={() =>
          openModal(<DeleteEmployee username={username} />, modalDispatch)
        }
      >
        Delete Employee
      </Button>

      <Divider />

      <FilterDateForm />

      <ListTimestamps
        username={username}
        pageSize={pageSize}
        forOneUser={true}
        showEditDelete={true}
      />
    </Fragment>
  );
};

export default ViewEmployee;
